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
        private readonly IShareService _shareService;
        private readonly Random randy;
        private readonly IMapper _mapper;
        private readonly IDatabaseSettings _dbSettings;
        private readonly IMongoCollection<EconomyResource> _economy;
        private readonly IMongoCollection<MarketResource> _market;
        public bool isRunning { get; set; }

        public MarketService(IDatabaseSettings settings, IAssetService assetService, IPlayerService playerService, IShareService shareService, IMapper mapper, IDatabaseSettings dbSettings)
        {
            _dbSettings = dbSettings;
            _mapper = mapper;
            randy = new Random();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            database.DropCollection(settings.EconomyCollectionName);
            _market = database.GetCollection<MarketResource>(settings.MarketCollectionName);
            _economy = database.GetCollection<EconomyResource>(settings.EconomyCollectionName);
            _assetService = assetService;
            _playerService = playerService;
            _shareService = shareService;
        }
        public ChartPixel GetRecords(Guid gameId, int lastSequence)
        {
            var query = _market.AsQueryable().Where(m => (m.SequenceNumber > lastSequence) && (m.GameId == gameId)).OrderByDescending(m => m.SequenceNumber).ToList();

            foreach (var item in query) Console.WriteLine(item.SequenceNumber);
            Console.WriteLine("that's it: " + query.Count);
            var pixel = new ChartPixel();
            if(query.Count > 0)
            {
                pixel.Open = query.LastOrDefault().Assets[0].Value;
                pixel.Close = query.FirstOrDefault().Assets[0].Value;
                pixel.LastFrame = query.FirstOrDefault().SequenceNumber;
                var ascendingValue = query.OrderBy(m => m.Assets[0].Value);
                pixel.High = ascendingValue.LastOrDefault().Assets[0].Value;
                pixel.Low = ascendingValue.FirstOrDefault().Assets[0].Value;
                pixel.Volume = query.Count;
            }
            return pixel;
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
        public void StartStop(Guid gameId, bool running)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var update = Builders<EconomyResource>.Update.Set("isRunning", running);
            var game = _economy.FindOneAndUpdate(filter, update);
            Motion(game.GameId);
        }
        public string BigBang(string secretCode)
        {
            if (secretCode == _dbSettings.Destructo)
            {
                _playerService.MassDestruction();
                return "the screams of the data as they were being deleted are starting to fade...";
            }
            return "Not gonna do it";
        }
        public async Task<string> EndGame(Guid gameId)
        {
            // Stop game
            var econFilter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var econUpdate  = Builders<EconomyResource>.Update.Set("isRunning", false);
            var econ = _economy.FindOneAndUpdateAsync(econFilter,econUpdate).Result;
            // destroy assets and their shares
            var assets = _assetService.GetAsync(gameId).Result;
            await assets.ForEachAsync(a =>
            {
                _shareService.ShredShares(Guid.Parse(a.AssetId));
                var assetFilter = Builders<AssetResource>.Filter.Eq("GameId", gameId);
                _assetService.RemoveFromGame(assetFilter);
            });
            // get the markets
            var marketsFilter = Builders<MarketResource>.Filter.Eq("GameId", gameId);
            var markets = _market.DeleteMany(marketsFilter);
            // players' turns
            var players = _playerService.RemovePlayersFromGame(gameId);
            var game = _economy.FindOneAndDeleteAsync(econFilter).Result;
            return "there is nothing left; just the rubble of bits and bytes to be reallocated";
        }
        public async Task<bool> IsRunning(Guid gameId)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var incoming = _economy.FindAsync(filter).Result;
            var isRunning = false;
            await incoming.ForEachAsync(g => isRunning = g.isRunning);
            return isRunning;
        }
        public Guid NewGame()
        {
            var newGame = new Economy();
            newGame.HAUS = _mapper.Map<PlayerResource,Player>(_playerService.Create(new Player("HAUS", Guid.NewGuid(), newGame.GameId)));
            newGame.CASH = _mapper.Map<AssetResource,Asset>(_assetService.Create(new Asset(ModelTypes.Cash.ToString(), Guid.NewGuid(), newGame.GameId)));
            _economy.InsertOneAsync(_mapper.Map<Economy,EconomyResource>(newGame));
            return newGame.GameId;
        }
        public async void Motion(Guid econId)
        {
            var econFilter = Builders<EconomyResource>.Filter.Eq("GameId", econId);
            var economy = new EconomyResource();
            var keepGoing = false;
            do
            {
                keepGoing = false;
                var markets = _market.AsQueryable().Where(m => m.GameId == econId).ToList();
                var incomingEconomy = _economy.FindAsync(econFilter).Result;
                await incomingEconomy.ForEachAsync(e => economy = e);
                if (!economy.isRunning) goto LoopEnd;
                var lastMarket = new Market(econId, economy.Assets, _mapper.Map<MarketResource,MarketMetrics>(markets.LastOrDefault()), randy);
                var lastMarketMetrics = lastMarket.GetMetrics(GrowAssets(lastMarket.Assets, lastMarket));
                var nextMarket = new Market(econId, economy.Assets, lastMarketMetrics, randy);
                nextMarket.SequenceNumber = lastMarket.SequenceNumber + 1;
                var nextMarketMetrics = nextMarket.GetMetrics(GrowAssets(lastMarket.Assets, lastMarket));
                _market.InsertOne(_mapper.Map<Market,MarketResource>(nextMarket));
                // process players' turns

                // finalizing
                keepGoing = await IsRunning(economy.GameId);
                Console.WriteLine("next market sequence number: " + nextMarketMetrics.SequenceNumber);
                Thread.Sleep(50);
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
        string BigBang(string secretCode);
        Task<string> EndGame(Guid gameId);
        ChartPixel GetRecords(Guid gameId, int lastSequence);
        void SetPixelCount(Guid gameId, int count);
        void SetTrendiness(Guid gameId, int trend);
        void StartStop(Guid gameId, bool running);
        Task<bool> IsRunning(Guid gameId);
        Guid NewGame();
        void Motion(Guid econId);
        public List<CompanyAsset> GetCompanyAssets(Guid gameId); Task<Economy> GetGame(Guid gameId);
        string UpdateGame(Economy game);
    }
}
