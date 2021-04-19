using AutoMapper;
using MongoDB.Driver;
using RiskGame.API.Entities;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using RiskGame.API.Persistence;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Logic
{
    public class EconLogic : IEconLogic
    {
        private readonly IAssetService _assetService;
        private readonly IPlayerService _playerService;
        private readonly IShareService _shareService;
        private readonly IMapper _mapper;
        private readonly Random randy;
        private readonly IDatabaseSettings _dbSettings;
        private readonly IMongoCollection<EconomyResource> _economy;
        private readonly IMongoCollection<MarketResource> _market;
        public EconLogic(IDatabaseSettings dbSettings, IAssetService assetService, IPlayerService playerService, IShareService shareService, IMapper mapper)
        {
            _dbSettings = dbSettings;
            _assetService = assetService;
            _playerService = playerService;
            _shareService = shareService;
            _mapper = mapper;
            randy = new Random();
            var client = new MongoClient(_dbSettings.ConnectionString);
            var database = client.GetDatabase(_dbSettings.DatabaseName);
            database.DropCollection(_dbSettings.EconomyCollectionName);
            _market = database.GetCollection<MarketResource>(_dbSettings.MarketCollectionName);
            _economy = database.GetCollection<EconomyResource>(_dbSettings.EconomyCollectionName);
        }
        public async Task<MarketLoopData> LoopRound(MarketLoopData precursors)
        {
            precursors.KeepGoing = false;
            var assets = _assetService.GetGameAssets(precursors.EconId).Where(a => a.CompanyAsset != null).ToArray();
            var companyAssets = assets.Select(a => a.CompanyAsset).ToArray();
            var lastMarketResource = _market.AsQueryable().Where(m => m.GameId == precursors.EconId).ToList().LastOrDefault();
            precursors.Economy = _economy.AsQueryable().Where(e => e.GameId == precursors.EconId).ToList().FirstOrDefault();
            var lastMarket = _mapper.Map<MarketResource,Market>(lastMarketResource);
            var lastMarketMetrics = lastMarket.GetMetrics(GrowAssets(assets, lastMarket).Select(a => a.CompanyAsset).ToArray());
            var nextMarket = new Market(precursors.EconId, companyAssets, randy, lastMarketMetrics);
            precursors.Market = lastMarketMetrics;
            _market.InsertOne(_mapper.Map<Market, MarketResource>(nextMarket));
            // process players' turns
            // finalizing
            precursors.KeepGoing = await IsRunning(precursors.EconId);
            //Thread.Sleep(1);
            return precursors;
        }
        private AssetResource[] GrowAssets(AssetResource[] assets, Market market)
        {
            foreach (var asset in assets)
            {
                if (asset.CompanyAsset == null) continue;
                var period = randy.Next(10, 100);
                var magnitude = GrowthRate(
                        market.GetMetric(asset.CompanyAsset.PrimaryIndustry),
                        market.GetMetric(asset.CompanyAsset.SecondaryIndustry)
                    ) / period;
                asset.CompanyAsset.Waves.Add(new Wave {
                    Magnitude = magnitude, 
                    Period = period });
                double growthRate = 0;
                foreach(var wave in asset.CompanyAsset.Waves)
                {
                    growthRate += wave.Magnitude;
                    wave.Period--;
                }
                asset.CompanyAsset.Waves = asset.CompanyAsset.Waves.Where(c => c.Period > 0).ToList();
                var value = asset.CompanyAsset.Value * (1 + growthRate);
                asset.CompanyAsset.Value = value;
                asset.History.Add(value);
                _assetService.Replace(Guid.Parse(asset.AssetId), _mapper.Map<AssetResource, Asset>(asset));
            }
            return assets;
        }
        private double GrowthRate(double primaryIndustryGrowth, double secondaryIndustryGrowth) => (7 * primaryIndustryGrowth - 3 * secondaryIndustryGrowth) / 100;
        public async Task<bool> IsRunning(Guid gameId)
        {
            var econ = _economy.AsQueryable().Where(e => e.GameId == gameId).Select(g => g.isRunning);
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var incoming = _economy.FindAsync(filter).Result;
            var isRunning = false;
            await incoming.ForEachAsync(g => isRunning = g.isRunning);
            return isRunning;
        }
    }
    public interface IEconLogic
    {
        Task<MarketLoopData> LoopRound(MarketLoopData precursors);
        Task<bool> IsRunning(Guid gameId);
    }
}
