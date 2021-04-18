using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.MarketFolder
{
    public class MarketMetricsHistory
    {
        public int SequenceNumber { get; set; }
        public Guid GameId { get; set; }
        public List<double> Red { get; set; }
        public List<double> Orange { get; set; }
        public List<double> Yellow { get; set; }
        public List<double> Green { get; set; }
        public List<double> Blue { get; set; }
        public List<double> Violet { get; set; }
    }
}
