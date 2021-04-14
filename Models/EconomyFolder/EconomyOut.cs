using RiskGame.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.EconomyFolder
{
    public class EconomyOut
    {
        public Guid GameId { get; set; }
        public CompanyAsset[] Assets { get; set; }
        public bool isRunning { get; set; }
    }
}
