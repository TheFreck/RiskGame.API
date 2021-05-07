using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.TransactionFolder
{
    public class TransactionResource
    {
        public int Sequence { get; set; }
        public Guid TradeId { get; set; }
        public Guid GameId { get; set; }
        public Guid Buyer { get; set; }
        public Guid Seller { get; set; }
        public Guid Asset { get; set; }
        public decimal Price { get; set; }
        public DateTime TradeTime { get; set; }
        public decimal CompanyAssetValue { get; set; }
    }
}
