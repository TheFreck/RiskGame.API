using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Entities;

namespace RiskGame.API.Mappings
{
    public class AssetResourceToCompanyAsset : Profile
    {
        public AssetResourceToCompanyAsset()
        {
            CreateMap<AssetResource, CompanyAsset>();
        }
    }
}
