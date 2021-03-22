using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.SharesFolder;

namespace RiskGame.API.Entities
{
    public class AssetWithShares
    {
        public Asset Asset { get; set; }
        public List<Share> Shares { get; set; }
    }
}
