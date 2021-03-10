using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.AssetFolder
{
    public class AssetIn
    {
        public string Name;
        public string Id;
        public int SharesOutstanding;
        public int? BookValue;
        public int? RateOfReturn;
    }
}
