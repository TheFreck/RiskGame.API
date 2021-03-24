using RiskGame.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Entities
{
    public class TradeRecord
    {
        public ModelReference Buyer { get; set; }
        public ModelReference Asset { get; set; }
        public Guid ShareId { get; set; }
        public int Price { get; set; }
        public DateTime TradeTime { get; set; }
    }
}
