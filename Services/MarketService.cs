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

namespace RiskGame.API.Services
{
    public class MarketService : IMarketService
    {
        private readonly IAssetService _assetService;
        private readonly Random randy;
        private readonly IMapper _mapper;
        private readonly IMongoCollection<EconomyResource> _economy;
        private readonly IMongoCollection<MarketResource> _market;
        public bool isRunning { get; set; }

        public MarketService(IDatabaseSettings settings, IAssetService assetService, IMapper mapper)
        {
            _mapper = mapper;
            randy = new Random();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            database.DropCollection(settings.EconomyCollectionName);
            _market = database.GetCollection<MarketResource>(settings.EconomyCollectionName);
            _economy = database.GetCollection<EconomyResource>(settings.MarketCollectionName);
            _assetService = assetService;
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
            _economy.InsertOneAsync(_mapper.Map<Economy,EconomyResource>(newGame));
            return newGame.GameId;
        }
        public void Motion(Economy economy)
        {
            economy.Assets = _assetService.GetCompanyAssets(economy.GameId).Result.ToArray();
            var keepGoing = false;
            do
            {
                Console.WriteLine("let's go: " + DateTime.Now);
                // check assets
                var lastMarket = new Market(economy.Trendiness, economy.Assets, economy.Markets.LastOrDefault(), randy);
                var lastMarketMetrics = lastMarket.GetMetrics(GrowAssets(lastMarket.Assets, lastMarket));
                var nextMarket = new Market(economy.Trendiness, economy.Assets, economy.Markets.LastOrDefault(), randy);
                var nextMarketMetrics = nextMarket.GetMetrics(GrowAssets(lastMarket.Assets, lastMarket));
                economy.Assets = nextMarketMetrics.Assets;
                _market.InsertOne(_mapper.Map<Market,MarketResource>(nextMarket));
                var filter = Builders<EconomyResource>.Filter.Eq("GameId", economy.GameId);
                var thisEcon = _economy.FindAsync(filter).Result;
                var newEconomy = new EconomyResource();
                thisEcon.ForEachAsync(e => newEconomy = e);
                keepGoing = newEconomy.isRunning;
                // process players' turns
                Thread.Sleep(1000);
            } while (keepGoing);
            Console.WriteLine("Finito");
        }
        private CompanyAsset[] GrowAssets(CompanyAsset[] assets, Market market)
        {
            foreach (var asset in assets)
            {
                var value = asset.Value * GrowthRate(asset.Value, market.GetMetric(asset.PrimaryIndustry), market.GetMetric(asset.SecondaryIndustry));
                asset.Value = value;
            }
            return assets;
        }
        private double GrowthRate(double value, double primaryIndustryGrowth, double secondaryIndustryGrowth) => (1 + primaryIndustryGrowth * Math.Abs(secondaryIndustryGrowth) / 10000);
        public List<CompanyAsset> GetCompanyAssets(Guid gameId)
        {
            return _assetService.GetCompanyAssets(gameId).Result;
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
        public List<CompanyAsset> GetCompanyAssets(Guid gameId);
    }
}
