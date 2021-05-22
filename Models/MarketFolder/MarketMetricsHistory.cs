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
        public Guid GameId = Guid.NewGuid();
        public List<double> Red { get; set; }
        public List<double> Orange { get; set; }
        public List<double> Yellow { get; set; }
        public List<double> Green { get; set; }
        public List<double> Blue { get; set; }
        public List<double> Violet { get; set; }
        public MarketMetricsHistory()
        {
            Red = new List<double>() { 0 };
            Orange = new List<double>() { 0 };
            Yellow = new List<double>() { 0 };
            Green = new List<double>() { 0 };
            Blue = new List<double>() { 0 };
            Violet = new List<double>() { 0 };
        }
    }
}
