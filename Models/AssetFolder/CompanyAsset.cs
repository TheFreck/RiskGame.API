using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.AssetFolder
{
    public class CompanyAsset
    {
        public Guid AssetId { get; set; }
        public IndustryTypes PrimaryIndustry { get; set; }
        public IndustryTypes SecondaryIndustry { get; set; }
        public decimal Value { get; set; }
        public decimal InternalRateOfReturn { get; set; }
        public List<Wave> Waves { get; set; }
        public CompanyAsset()
        {
            var randy = new Random();
            Waves = new List<Wave>();
            Value = (decimal)randy.NextDouble() * 10000 + 10000;
            InternalRateOfReturn = (decimal)(1 + randy.NextDouble() / (randy.NextDouble() + 100));
        }
    }
}
