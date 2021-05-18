using MongoDB.Driver;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Entities
{
    public class MarketLoopData
    {
        [Required]
        public EconomyResource Economy { get; set; }
        //[Required]
        //public MarketMetrics MarketMetrics { get; set; }
        [Required]
        public Guid EconId { get; set; }
        [Required]
        public AssetResource[] Assets { get; set; }
        [Required]
        public MarketResource Market { get; set; }
    }
}
