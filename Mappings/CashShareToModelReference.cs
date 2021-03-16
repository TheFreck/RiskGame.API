using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models;
using RiskGame.API.Models.SharesFolder;

namespace RiskGame.API.Mappings
{
    public class CashShareToModelReference : Profile
    {
        public CashShareToModelReference()
        {
            CreateMap<CashShare, ModelReference>();
        }
    }
}
