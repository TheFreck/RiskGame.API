using RiskGame.API.Entities;
using RiskGame.API.Models.AssetFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.EconomyFolder
{
    public class EconomyOut
    {
        public Guid GameId { get; set; }
        public Guid[] Assets { get; set; }
        public string Message { get; set; }
    }
}
