using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models.MarketFolder;
using AutoMapper;
using RiskGame.API.Models.EconomyFolder;

namespace RiskGame.API.Mappings
{
    public class EconomyToEconomyResource : Profile
    {
        public EconomyToEconomyResource()
        {
            CreateMap<Economy, EconomyResource>();
        }
    }
    public class EconomyResourceToEconomy: Profile
    {
        public EconomyResourceToEconomy()
        {
            CreateMap<EconomyResource, Economy>();
        }
    }
}
