using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.EconomyFolder
{
    public class EconMetrics
    {
        public int SequenceNumber { get; set; }
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
    }
}
