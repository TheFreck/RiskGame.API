using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Entities.Enums
{
    public enum TurnTypes
    {
        QuickSell = -2,
        Sell = -1,
        Hold = 0,
        Buy = 1,
        QuickBuy = 2
    }
}
