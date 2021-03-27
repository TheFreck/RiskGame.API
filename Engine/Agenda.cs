using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Engine
{
    public class Agenda
    {
        public bool isOn { get; set; }
        private readonly Random randy;
        private readonly IAssetService _assetService;
        private CompanyAsset[] Assets;
        public List<EconMetrics> Economies;
        public Agenda(IAssetService assetService)
        {
            // set the agenda
            _assetService = assetService;
            Economies = new List<EconMetrics>()
            {
                new EconMetrics
                {
                    Red = 1,
                    Orange =1,
                    Yellow =1,
                    Green =1,
                    Blue =1,
                    Violet=1,
                    RedDirection = Direction.Up,
                    OrangeDirection =Direction.Up,
                    YellowDirection = Direction.Up,
                    GreenDirection =Direction.Up,
                    BlueDirection = Direction.Up,
                    VioletDirection =Direction.Up                }
            };
            randy = new Random();
        }
        public void Motion(int count, int trendiness)
        {
            Assets = _assetService.GetCompanyAssetsAsync().Result.ToArray();
            do
            {
                var economy = new Economy(trendiness, Assets, Economies[Economies.Count-1], randy);
                Economies.Add(economy.GetMetrics(GrowAssets(economy.Assets, economy)));
                count--;
                // process players' turns
            } while (count > 0);
            Console.WriteLine("Finito");
        }
        private CompanyAsset[] GrowAssets(CompanyAsset[] assets, Economy economy)
        {
            foreach(var asset in assets)
            {
                var value = asset.Value * GrowthRate(asset.Value, economy.GetMetric(asset.Industry), asset.Cyclicality);
                asset.Value = value;
            }
            return assets;
        }
        private double GrowthRate(double value, double industryGrowth, int cyclicality) => (1 + industryGrowth * cyclicality/100);

        // generate economic metrics
        // grow Company Assets
        // process players' turns
    }
}
