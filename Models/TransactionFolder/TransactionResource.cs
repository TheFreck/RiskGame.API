using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.TransactionFolder
{
    public class TransactionResource
    {
        public int Sequence { get; set; }
        public Guid TradeId { get; set; }
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public TradeType Action { get; set; }
        [Required]
        public Guid Buyer { get; set; }
        [Required]
        public Guid Seller { get; set; }
        [Required]
        public Guid Asset { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Shares { get; set; }
        [Required]
        public DateTime TradeTime { get; set; }
    }
}
