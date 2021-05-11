using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.PlayerFolder
{
    public class PlayerDecision
    {
        //
        // AssetAllocation() determines Qty
        // Qty < 0 = decision to sell
        // Qty > 0 = decision to buy
        // EvaluateAsset() determines the Price willing to pay
        // Grapevine() adds an enhancement of QuickBuy or QuickSell
        public ModelReference Asset { get; set; }
        public TurnTypes Action { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
    }
}
