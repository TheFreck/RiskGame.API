using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.AssetFolder
{
    public class AssetIn
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string GameId { get; set; }
        public int SharesOutstanding { get; set; }
    }
}
