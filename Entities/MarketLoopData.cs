using MongoDB.Driver;
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
        public FilterDefinition<EconomyResource> Filter { get; set; }
        [Required]
        public EconomyResource Economy { get; set; }
        public MarketMetrics Market { get; set; }
        [Required]
        public bool KeepGoing { get; set; }
        [Required]
        public Guid EconId { get; set; }
    }
}
