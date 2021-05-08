using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.TransactionFolder
{
    public class TradeClient
    {
        public Guid TradeId { get; set; }
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public Guid Buyer { get; set; }
        [Required]
        public Guid Seller { get; set; }
        [Required]
        public Guid Asset { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime TradeTime { get; set; }
        public decimal CompanyAssetValue { get; set; }
    }
}
