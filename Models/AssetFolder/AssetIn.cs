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
        public int Leverage { get; set; }
        public int AnimalSpirits { get; set; }
        public int Income { get; set; }
        public int RiskStrategy { get; set; }
        public int Cyclicality { get; set; }
    }
}
