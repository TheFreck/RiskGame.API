using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Entities;
using RiskGame.API.Models.PlayerFolder;
using MongoDB.Driver;
using RiskGame.API.Persistence;
using RiskGame.API.Models.EconomyFolder;
using AutoMapper;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Mappings;
using RiskGame.API.Models.MarketFolder;
using MongoDB.Bson;
using System.Threading;
using RiskGame.API.Entities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace RiskGame.API.Services
{
    public class MarketService : IMarketService
    {
        private readonly IAssetService _assetService;
        private readonly IPlayerService _playerService;
        private readonly Random randy;
        private readonly IMapper _mapper;
        private readonly IMongoCollection<EconomyResource> _economy;
        private readonly IMongoCollection<MarketResource> _market;
        public bool isRunning { get; set; }

        public MarketService(IDatabaseSettings settings, IAssetService assetService, IPlayerService playerService, IMapper mapper)
        {
            _mapper = mapper;
            randy = new Random();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            database.DropCollection(settings.EconomyCollectionName);
            _market = database.GetCollection<MarketResource>(settings.MarketCollectionName);
            _economy = database.GetCollection<EconomyResource>(settings.EconomyCollectionName);
            _assetService = assetService;
            _playerService = playerService;
        }
        public async Task<List<MarketMetrics>> GetRecords(Guid gameId)
        {
            var incomingMarkets = await _market.FindAsync(e => e.SequenceNumber >= 0);
            var markets = new List<MarketMetrics>();
            await incomingMarkets.ForEachAsync(e => markets.Add(_mapper.Map<MarketResource, MarketMetrics>(e)));
            return markets.OrderBy(e => e.SequenceNumber).ToList();
        }
        public void SetPixelCount(Guid gameId, int count) {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var update = Builders<EconomyResource>.Update.Set("PixelCount", count);
            _economy.UpdateOne(filter, update);
        }
        public void SetTrendiness(Guid gameId, int trend)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var update = Builders<EconomyResource>.Update.Set("Trendiness", trend);
            _economy.UpdateOne(filter, update);
        }
        public void StartStop(Guid gameId)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var incoming = _economy.FindAsync(filter).Result;
            var game = new EconomyResource();
            
            incoming.ForEachAsync(g => game = g);
            game.isRunning = !game.isRunning;
            var update = Builders<EconomyResource>.Update.Set("isRunning", game.isRunning);
            _economy.UpdateOne(filter, update);
            Motion(_mapper.Map<EconomyResource,Economy>(game));
        }
        public async Task<bool> IsRunning(Guid gameId)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var incoming = _economy.FindAsync(filter).Result;
            var game = new EconomyResource();
            await incoming.ForEachAsync(g => game = g);
            return game.isRunning;
        }
        public Guid NewGame()
        {
            var newGame = new Economy();
            newGame.HAUS = _mapper.Map<PlayerResource,Player>(_playerService.Create(new Player("HAUS", Guid.NewGuid(), newGame.GameId)));
            newGame.CASH = _mapper.Map<AssetResource,Asset>(_assetService.Create(new Asset(ModelTypes.Cash.ToString(), Guid.NewGuid(), newGame.GameId)));
            _economy.InsertOneAsync(_mapper.Map<Economy,EconomyResource>(newGame));
            return newGame.GameId;
        }
        public async void Motion(Economy economy)
        {
            var keepGoing = false;
            do
            {
                if (!economy.isRunning) goto LoopEnd;
                Console.WriteLine("let's go: " + DateTime.Now);
                var lastMarket = new Market(economy.GameId, economy.Assets, economy.Markets.LastOrDefault(), randy);
                var lastMarketMetrics = lastMarket.GetMetrics(GrowAssets(lastMarket.Assets, lastMarket));
                var nextMarket = new Market(economy.GameId, economy.Assets, economy.Markets.LastOrDefault(), randy);
                var nextMarketMetrics = nextMarket.GetMetrics(GrowAssets(lastMarket.Assets, lastMarket));
                _market.InsertOne(_mapper.Map<Market,MarketResource>(nextMarket));
                var filter = Builders<EconomyResource>.Filter.Eq("GameId", economy.GameId);
                var incomingEcon = _economy.FindAsync(filter).Result;
                var newEconomy = new Economy();
                var newEconomyResource = new EconomyResource();
                newEconomy = _mapper.Map<EconomyResource, Economy>(newEconomyResource);
                await incomingEcon.ForEachAsync(e => newEconomyResource = e);
                newEconomyResource.Assets = nextMarketMetrics.Assets;
                newEconomyResource.Markets.Add(nextMarketMetrics);
                _economy.ReplaceOne(filter, newEconomyResource);
                // process players' turns
                keepGoing = await IsRunning(economy.GameId);
                Thread.Sleep(500);
            } while (keepGoing);
        LoopEnd:
            Console.WriteLine("Finito");
            return;
        }
        private CompanyAsset[] GrowAssets(CompanyAsset[] assets, Market market)
        {
            foreach (var asset in assets)
            {
                var value = asset.Value * GrowthRate(asset.Value, market.GetMetric(asset.PrimaryIndustry), market.GetMetric(asset.SecondaryIndustry));
                asset.Value = value;
            }
            // SAVE ASSETS AFTER UPDATE
            return assets;
        }
        private double GrowthRate(double value, double primaryIndustryGrowth, double secondaryIndustryGrowth) => (1 + primaryIndustryGrowth * Math.Abs(secondaryIndustryGrowth) / 10000);
        public List<CompanyAsset> GetCompanyAssets(Guid gameId)
        {
            return _assetService.GetCompanyAssets(gameId).Result;
        }
        public async Task<Economy> GetGame(Guid gameId)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var incoming = _economy.FindAsync(filter).Result;
            var game = new EconomyResource();
            await incoming.ForEachAsync(g => game = g);
            return _mapper.Map<EconomyResource,Economy>(game);
        }
        public string UpdateGame(Economy game)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", game.GameId);
            try
            {
                return _economy.ReplaceOne(filter,_mapper.Map<Economy,EconomyResource>(game)).ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
    public interface IMarketService
    {
        Task<List<MarketMetrics>> GetRecords(Guid gameId);
        void SetPixelCount(Guid gameId, int count);
        void SetTrendiness(Guid gameId, int trend);
        void StartStop(Guid gameId);
        Task<bool> IsRunning(Guid gameId);
        Guid NewGame();
        void Motion(Economy economy);
        public List<CompanyAsset> GetCompanyAssets(Guid gameId); Task<Economy> GetGame(Guid gameId);
        string UpdateGame(Economy game);
    }
}
