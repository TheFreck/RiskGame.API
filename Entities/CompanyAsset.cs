using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Entities
{
    public class CompanyAsset
    {
        public int Cyclicality { get; set; }
        public IndustryTypes Industry { get; set; }
        public double Value { get; set; }
    }
}
