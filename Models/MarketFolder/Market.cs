using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.MarketFolder
{
    public class Market
    {
        public int SequenceNumber { get; set; }
        public Guid GameId { get; set; }
        public int MarketTrendiness; // 0-9
        public DateTime Time { get; set; }
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
        public Player HAUS { get; set; }
        public Asset CASH { get; set; }
        public Market(Guid gameId, CompanyAsset[] assets, Random randy, MarketMetrics yester)
        {
            Time = DateTime.Now;
            SequenceNumber = yester != null ? yester.SequenceNumber + 1 : 0;
            GameId = gameId;
            Assets = assets;
            MarketTrendiness = randy.Next(8);
            if (randy.Next(9) > MarketTrendiness) RedDirection = yester != null ? (Direction)((int)yester.RedDirection * -1) : Direction.Up;
            else RedDirection = yester != null ? yester.RedDirection : Direction.Up;
            Red = (int)RedDirection * (.01 / randy.NextDouble() - .009);
            if (randy.Next(9) > MarketTrendiness) OrangeDirection = yester != null ? (Direction)((int)yester.OrangeDirection * -1) : Direction.Up;
            else OrangeDirection = yester != null ? yester.OrangeDirection : Direction.Up;
            Orange = (int)OrangeDirection * (.01 / randy.NextDouble() - .009);
            if (randy.Next(9) > MarketTrendiness) YellowDirection = yester != null ? (Direction)((int)yester.YellowDirection * -1) : Direction.Up;
            else YellowDirection = yester != null ? yester.YellowDirection : Direction.Up;
            Yellow = (int)YellowDirection * (.01 / randy.NextDouble() - .009);
            if (randy.Next(9) > MarketTrendiness) GreenDirection = yester != null ? (Direction)((int)yester.GreenDirection * -1) : Direction.Up;
            else GreenDirection = yester != null ? yester.GreenDirection : Direction.Up;
            Green = (int)GreenDirection * (.01 / randy.NextDouble() - .009);
            if (randy.Next(9) > MarketTrendiness) BlueDirection = yester != null ? (Direction)((int)yester.BlueDirection * -1) : Direction.Up;
            else BlueDirection = yester != null ? yester.BlueDirection : Direction.Up;
            Blue = (int)BlueDirection * (.01 / randy.NextDouble() - .009);
            if (randy.Next(9) > MarketTrendiness) VioletDirection = yester != null ? (Direction)((int)yester.VioletDirection * -1) : Direction.Up;
            else VioletDirection = yester != null ? yester.VioletDirection : Direction.Up;
            Violet = (int)VioletDirection * (.01 / randy.NextDouble() - .009);
        }
        public Market()
        {
            Red = Orange = Yellow = Green = Blue = Violet = 0;
            RedDirection = OrangeDirection = YellowDirection = GreenDirection = BlueDirection = VioletDirection = Direction.Up;
        }
        public MarketMetrics GetMetrics()
        {
            return new MarketMetrics
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
        public MarketMetrics GetMetrics(CompanyAsset[] assets)
        {
            return new MarketMetrics
            {
                GameId = GameId,
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
        public double GetAverageMetric() => (Red + Orange + Yellow + Green + Blue + Violet) / 6;
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
}
