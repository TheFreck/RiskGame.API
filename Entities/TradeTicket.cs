using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Entities
{
    public class TradeTicket
    {
        [Required]
        public ModelReference Buyer { get; set; }
        //[Required]
        public ModelReference Seller { get; set; }
        [Required]
        public ModelReference Asset { get; set; }
        [Required]
        public ModelReference[] Shares { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public DateTime TradeTime { get; set; }
        public bool SuccessfulTrade { get; set; }
        public string Message { get; set; }
    }
}
