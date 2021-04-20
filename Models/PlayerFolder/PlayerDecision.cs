using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.PlayerFolder
{
    public class PlayerDecision
    {
        public TurnTypes Allocation { get; set; }
        public TurnTypes Grapevine { get; set; }
        public TurnTypes Asset { get; set; }
        public double Value { get; set; }
        public int Qty { get; set; }
    }
}
