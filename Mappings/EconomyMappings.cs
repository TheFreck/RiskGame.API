using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models.EconomyFolder;
using AutoMapper;

namespace RiskGame.API.Mappings
{
    public class EconomyMappings : Profile
    {
        public EconomyMappings()
        {
            CreateMap<EconomyResource, EconomyOut>();
        }
    }
}
