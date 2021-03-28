using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.EconomyFolder
{
    public class Economy
    {
        public int SequenceNumber { get; set; }
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
            SequenceNumber = yester.SequenceNumber + 1;
            Assets = assets;
            MarketTrendiness = marketTrendiness;
            if (randy.Next(10) > marketTrendiness) RedDirection = (Direction)((int)yester.RedDirection * -1);
            else RedDirection = yester.RedDirection;
            Red = randy.NextDouble() * (int)RedDirection;
            if (randy.Next(10) > marketTrendiness) OrangeDirection = (Direction)((int)yester.OrangeDirection * -1);
            else OrangeDirection = yester.OrangeDirection;
            Orange = randy.NextDouble() * (int)OrangeDirection;
            if (randy.Next(10) > marketTrendiness) YellowDirection = (Direction)((int)yester.YellowDirection * -1);
            else YellowDirection = yester.YellowDirection;
            Yellow = randy.NextDouble() * (int)YellowDirection;
            if (randy.Next(10) > marketTrendiness) GreenDirection = (Direction)((int)yester.GreenDirection * -1);
            else GreenDirection = yester.GreenDirection;
            Green = randy.NextDouble() * (int)GreenDirection;
            if (randy.Next(10) > marketTrendiness) BlueDirection = (Direction)((int)yester.BlueDirection * -1);
            else BlueDirection = yester.BlueDirection;
            Blue = randy.NextDouble() * (int)BlueDirection;
            if (randy.Next(10) > marketTrendiness) VioletDirection = (Direction)((int)yester.VioletDirection * -1);
            else VioletDirection = yester.VioletDirection;
            Violet = randy.NextDouble() * (int)VioletDirection;
        }
        public EconMetrics GetMetrics()
        {
            return new EconMetrics
            {
                SequenceNumber = SequenceNumber,
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
                SequenceNumber = SequenceNumber,
                Red = Red,
                RedDirection = RedDirection,
                Orange = Orange,
                OrangeDirection = OrangeDirection,
                Yellow = Yellow,
                YellowDirection = YellowDirection,
                Green = Green,
                GreenDirection = GreenDirection,
                Blue = Blue,
                BlueDirection = BlueDirection,
                Violet = Violet,
                VioletDirection = VioletDirection,
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
