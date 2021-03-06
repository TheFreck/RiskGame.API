using RiskGame.API.Entities.Enums;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.TransactionFolder
{
    public class TradeTicket
    {
        public Guid GameId { get; set; }
        public Guid TradeId { get; set; }
        public TradeType Action { get; set; }
        public ModelReference  Buyer { get; set; }
        public ModelReference  Seller { get; set; }
        public ModelReference  Asset { get; set; }
        public int Shares { get; set; }
        public decimal Cash { get; set; }
        public DateTime TradeTime { get; set; }
        public bool SuccessfulTrade { get; set; }
        public string Message { get; set; }
    }
}
