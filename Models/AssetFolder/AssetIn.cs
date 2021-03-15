using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.AssetFolder
{
    public class AssetIn
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int SharesOutstanding { get; set; }
        public int? BookValue { get; set; }
        public int? RateOfReturn { get; set; }
    }
}
