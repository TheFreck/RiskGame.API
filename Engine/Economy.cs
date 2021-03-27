using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Engine
{
    public class Economy
    {
        private int MarketTrendiness; // 0-9
        public double Red { get; set; }
        public Direction RedDirection;
        public double Orange { get; set; }
        public Direction OrangeDirection;
        public double Yellow { get; set; }
        public Direction YellowDirection;
        public double Green { get; set; }
        public Direction GreenDirection;
        public double Blue { get; set; }
        public Direction BlueDirection;
        public double Violet { get; set; }
        public Direction VioletDirection;
        public CompanyAsset[] Assets { get; set; }
        public Player[] Players { get; set; }
        public Economy(int marketTrendiness, CompanyAsset[] assets, EconMetrics yester, Random randy)
        {
            RedDirection = yester.RedDirection;
            OrangeDirection = yester.OrangeDirection;
            YellowDirection = yester.YellowDirection;
            GreenDirection = yester.GreenDirection;
            BlueDirection = yester.BlueDirection;
            VioletDirection = yester.VioletDirection;

            Assets = assets;
            MarketTrendiness = marketTrendiness;
            if (randy.Next(10) > MarketTrendiness) RedDirection = (Direction)((int)RedDirection * -1);
            Red = randy.NextDouble() * (int)RedDirection;
            if (randy.Next(10) > MarketTrendiness) OrangeDirection = (Direction)((int)OrangeDirection * -1);
            Orange = randy.NextDouble() * (int)OrangeDirection;
            if (randy.Next(10) > MarketTrendiness) YellowDirection = (Direction)((int)YellowDirection * -1);
            Yellow = randy.NextDouble() * (int)YellowDirection;
            if (randy.Next(10) > MarketTrendiness) GreenDirection = (Direction)((int)GreenDirection * -1);
            Green = randy.NextDouble() * (int)GreenDirection;
            if (randy.Next(10) > MarketTrendiness) BlueDirection = (Direction)((int)BlueDirection * -1);
            Blue = randy.NextDouble() * (int)BlueDirection;
            if (randy.Next(10) > MarketTrendiness) VioletDirection = (Direction)((int)VioletDirection * -1);
            Violet = randy.NextDouble() * (int)VioletDirection;
        }
        public EconMetrics GetMetrics()
        {
            return new EconMetrics
            {
                Red = Red,
                Orange = Orange,
                Yellow = Yellow,
                Green = Green,
                Blue = Blue,
                Violet = Violet,
            };
        }
        public EconMetrics GetMetrics(CompanyAsset[] assets)
        {
            return new EconMetrics
            {
                Red = Red,
                Orange = Orange,
                Yellow = Yellow,
                Green = Green,
                Blue = Blue,
                Violet = Violet,
                Assets = assets
            };
        }
        public double GetMetric() => (Red + Orange + Yellow + Green + Blue + Violet) / 6;
        public double GetMetric(IndustryTypes type)
        {
            switch (type)
            {
                case IndustryTypes.Red:
                    return Red;
                case IndustryTypes.Orange:
                    return Orange;
                case IndustryTypes.Yellow:
                    return Yellow;
                case IndustryTypes.Green:
                    return Green;
                case IndustryTypes.Blue:
                    return Blue;
                case IndustryTypes.Violet:
                    return Violet;
                case IndustryTypes.Average:
                    // returns the market average
                    return (Red + Orange + Yellow + Green + Blue + Violet) / 6;
            }
            return -1;
        }
        public Direction[] GetDirections()
        {
            return new Direction[]
            {
                RedDirection,
                OrangeDirection,
                YellowDirection,
                GreenDirection,
                BlueDirection,
                VioletDirection
            };
        }
    }
    public interface IEconomy
    {
        EconMetrics GetMetrics();
        EconMetrics GetMetrics(CompanyAsset[] assets);
        double GetMetric();
        double GetMetric(IndustryTypes type);
        Direction[] GetDirections();
    }
}
