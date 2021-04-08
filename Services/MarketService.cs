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
        public ChartPixel GetRecords(Guid gameId, int lastSequence)
        {
            var query = _market.AsQueryable().Where(m => m.SequenceNumber >= lastSequence).OrderByDescending(m => m.SequenceNumber).ToList();
            var ascendingValue = query.OrderBy(m => m.Assets[0].Value);
            return new ChartPixel
            {
                Open = query[query.Count - 1].Assets[0].Value,
                Close = query[0].Assets[0].Value,
                High = ascendingValue.FirstOrDefault().Assets[0].Value,
                Low = ascendingValue.LastOrDefault().Assets[0].Value,
                Volume = query.Count,
                LastFrame = query[0].SequenceNumber
            };


            
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
            var sequenceNumber = 0;
            do
            {
                Console.WriteLine("sequence number: " + sequenceNumber);
                if (!economy.isRunning) goto LoopEnd;
                var lastMarket = new Market(economy.GameId, economy.Assets, economy.Markets.LastOrDefault(), randy);
                var lastMarketMetrics = lastMarket.GetMetrics(GrowAssets(lastMarket.Assets, lastMarket));
                var nextMarket = new Market(economy.GameId, economy.Assets, economy.Markets.LastOrDefault(), randy);
                nextMarket.SequenceNumber = sequenceNumber;
                sequenceNumber++;
                var nextMarketMetrics = nextMarket.GetMetrics(GrowAssets(lastMarket.Assets, lastMarket));
                _market.InsertOne(_mapper.Map<Market,MarketResource>(nextMarket));
                var filter = Builders<EconomyResource>.Filter.Eq("GameId", economy.GameId);
                var incomingEcon = _economy.FindAsync(filter).Result;
                var newEconomyResource = new EconomyResource();
                await incomingEcon.ForEachAsync(e => newEconomyResource = e);
                newEconomyResource.Assets = nextMarketMetrics.Assets;
                newEconomyResource.Markets.Add(nextMarketMetrics);
                _economy.ReplaceOne(filter, newEconomyResource);
                // process players' turns

                // finalizing
                keepGoing = await IsRunning(economy.GameId);
                Console.WriteLine("next market sequence number: " + nextMarketMetrics.SequenceNumber);
                Thread.Sleep(100);
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
        private double GrowthRate(double value, double primaryIndustryGrowth, double secondaryIndustryGrowth)
        {
            var growthRate = 1 + (7 * primaryIndustryGrowth - 3 * secondaryIndustryGrowth) / 10000;
            return growthRate;
        }
        

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
        ChartPixel GetRecords(Guid gameId, int lastSequence);
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
