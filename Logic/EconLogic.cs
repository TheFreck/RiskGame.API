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
        public EconLogic(IAssetService assetService, IPlayerService playerService, IShareService shareService, IMapper mapper)
        {
            _assetService = assetService;
            _playerService = playerService;
            _shareService = shareService;
            _mapper = mapper;
            randy = new Random();
        }
        public async Task<MarketLoopData> LoopRound(MarketLoopData precursors)
        {
            precursors.KeepGoing = false;
            var assets = _assetService.GetGameAssets(precursors.EconId).Where(a => a.CompanyAsset != null).ToArray();
            var companyAssets = assets.Select(a => a.CompanyAsset).ToArray();
            var lastMarketResource = _market.AsQueryable().Where(m => m.GameId == precursors.EconId).LastOrDefault();
            precursors.Economy = _economy.AsQueryable().Where(e => e.GameId == precursors.EconId).FirstOrDefault();
            var lastMarket = _mapper.Map<MarketResource,Market>(lastMarketResource);
            var lastMarketMetrics = lastMarket.GetMetrics(_mapper.Map<AssetResource[], CompanyAsset[]>(GrowAssets(assets, lastMarket)));
            var nextMarket = new Market(precursors.EconId, companyAssets, lastMarketMetrics, randy);
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
                var value = asset.CompanyAsset.Value * GrowthRate(market.GetMetric(asset.CompanyAsset.PrimaryIndustry), market.GetMetric(asset.CompanyAsset.SecondaryIndustry));
                //Console.WriteLine("asset value: " + value);
                asset.CompanyAsset.Value = value;
                _assetService.Replace(Guid.Parse(asset.AssetId), _mapper.Map<AssetResource, Asset>(asset));
            }
            return assets;
        }
        private double GrowthRate(double primaryIndustryGrowth, double secondaryIndustryGrowth)
        {
            var backhalf = (7 * primaryIndustryGrowth - 3 * secondaryIndustryGrowth) / 100;
            var growthRate = 1.001 + backhalf;
            //Console.WriteLine("growth rate: " + growthRate);
            return growthRate;
        }
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
