using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Logic
{
    public class Economy : IEconomy
    {
        private int MarketTrendiness; // 0-9
        private Random randy;
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
        public Economy(int marketTrendiness)
        {
            MarketTrendiness = marketTrendiness;
            randy = new Random();
        }
        public void GenerateEconomicData()
        {
            MarketTrendiness = randy.Next(10);
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
                Violet = Violet
            };
        }
    }
    public interface IEconomy
    {
        void GenerateEconomicData();
        EconMetrics GetMetrics();
    }
}
