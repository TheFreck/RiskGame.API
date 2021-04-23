using AutoMapper;
using MongoDB.Driver;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
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
        private readonly IMapper _mapper;
        private readonly Random randy;
        public EconLogic(IDatabaseSettings dbSettings, IMapper mapper)
        {
            _mapper = mapper;
            randy = new Random();
        }
        public MarketLoopData LoopRound(MarketLoopData precursors)
        {
            // grow assets
            var lastMarketResource = precursors.LastMarket;
            var lastMarket = _mapper.Map<MarketResource,Market>(lastMarketResource);
            var grownAssets = GrowAssets(precursors.Assets, lastMarket).Select(a => a.CompanyAsset).ToArray();
            var lastMarketMetrics = lastMarket.GetMetrics(grownAssets);
            var nextMarket = new Market(precursors.EconId, precursors.Assets.Select(a => a.CompanyAsset).ToArray(), randy, lastMarketMetrics);
            // update economy
            precursors.Economy.Assets = grownAssets;
            // update precursors
            var red = nextMarket.GetMetric(IndustryTypes.Red);
            var orange = nextMarket.GetMetric(IndustryTypes.Orange);
            var yellow = nextMarket.GetMetric(IndustryTypes.Yellow);
            var green = nextMarket.GetMetric(IndustryTypes.Green);
            var blue = nextMarket.GetMetric(IndustryTypes.Blue);
            var violet = nextMarket.GetMetric(IndustryTypes.Violet);
            precursors.LastMarket = _mapper.Map<Market,MarketResource>(nextMarket);
            precursors.Economy.History.Red.Add(red);
            precursors.Economy.History.Orange.Add(orange);
            precursors.Economy.History.Yellow.Add(yellow);
            precursors.Economy.History.Green.Add(green);
            precursors.Economy.History.Blue.Add(blue);
            precursors.Economy.History.Violet.Add(violet);



            return precursors;
        }
        private AssetResource[] GrowAssets(AssetResource[] assets, Market market)
        {
            foreach (var asset in assets)
            {
                if (asset.CompanyAsset == null) continue;
                var period = randy.Next(10, 100);
                var primary = market.GetMetric(asset.CompanyAsset.PrimaryIndustry);
                var secondary = market.GetMetric(asset.CompanyAsset.SecondaryIndustry);
                var magnitude = GrowthRate(primary, secondary) / period;
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
                //_assetService.Replace(Guid.Parse(asset.AssetId), _mapper.Map<AssetResource, Asset>(asset));
            }
            return assets;
        }
        private double GrowthRate(double primaryIndustryGrowth, double secondaryIndustryGrowth) => (7 * primaryIndustryGrowth - 3 * secondaryIndustryGrowth) / 100;
    }
    public interface IEconLogic
    {
        MarketLoopData LoopRound(MarketLoopData precursors);
    }
}
