using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Entities
{
    public class ChartPixel
    {
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public int Volume { get; set; }
        public int LastFrame { get; set; }
        public DateTime TimeOpen { get; set; }
        public DateTime TimeClose { get; set; }
    }
}
