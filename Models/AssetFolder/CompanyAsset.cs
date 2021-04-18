using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.AssetFolder
{
    public class CompanyAsset
    {
        public IndustryTypes PrimaryIndustry { get; set; }
        public IndustryTypes SecondaryIndustry { get; set; }
        public double Value { get; set; }
    }
}
