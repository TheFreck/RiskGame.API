using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.MarketFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Entities
{
    public class Newspaper
    {
        private readonly MarketMetricsHistory _history;

        public Newspaper(MarketMetricsHistory history)
        {
            _history = history;
        }
        public Issue ReadNewspaper(Sophistication level, AssetResource asset)
        {
            var memory = level == Sophistication.Savvy ? 22 : 7;
            double primaryGrowth = 0;
            double primarySucessRatio = 0;
            double secondarySuccessRatio = 0;
            var red = _history.Red.TakeLast(memory).Average();
            var orange = _history.Orange.TakeLast(memory).Average();
            var yellow = _history.Yellow.TakeLast(memory).Average();
            var green = _history.Green.TakeLast(memory).Average();
            var blue = _history.Blue.TakeLast(memory).Average();
            var violet = _history.Violet.TakeLast(memory).Average();
            double marketGrowth = new double[]{red,orange,yellow,green,blue,violet}.Average();

            switch (asset.CompanyAsset.PrimaryIndustry)
            {
                case IndustryTypes.Red:
                    primaryGrowth = _history.Red.TakeLast(memory).Average();
                    primarySucessRatio = _history.Red.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Orange:
                    primaryGrowth = _history.Orange.TakeLast(memory).Average();
                    primarySucessRatio = _history.Orange.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Yellow:
                    primaryGrowth = _history.Yellow.TakeLast(memory).Average();
                    primarySucessRatio = _history.Yellow.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Green:
                    primaryGrowth = _history.Green.TakeLast(memory).Average();
                    primarySucessRatio = _history.Green.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Blue:
                    primaryGrowth = _history.Blue.TakeLast(memory).Average();
                    primarySucessRatio = _history.Blue.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Violet:
                    primaryGrowth = _history.Violet.TakeLast(memory).Average();
                    primarySucessRatio = _history.Violet.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
            }
            switch (asset.CompanyAsset.SecondaryIndustry)
            {
                case IndustryTypes.Red:
                    secondarySuccessRatio = _history.Red.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Orange:
                    secondarySuccessRatio = _history.Orange.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Yellow:
                    secondarySuccessRatio = _history.Yellow.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Green:
                    secondarySuccessRatio = _history.Green.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Blue:
                    secondarySuccessRatio = _history.Blue.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
                case IndustryTypes.Violet:
                    secondarySuccessRatio = _history.Violet.TakeLast(memory).Count(d => d > 0) / memory;
                    break;
            }
            return new Issue
            {
                PrimaryGrowth = primaryGrowth,
                PrimarySuccessRatio = primarySucessRatio,
                SecondarySuccessRatio = secondarySuccessRatio,
                MarketGrowth = marketGrowth,
                LastDividendValue = (double)asset.LastDividendPayout,
                PeriodsSinceDividend = asset.PeriodsSinceDividend
            };
        }
    }
    public class Issue
    {
        public double PrimaryGrowth { get; set; }
        public double PrimarySuccessRatio { get; set; }
        public double SecondarySuccessRatio { get; set; }
        public double MarketGrowth { get; set; }
        public double LastDividendValue { get; set; }
        public int PeriodsSinceDividend { get; set; }
    }
}
