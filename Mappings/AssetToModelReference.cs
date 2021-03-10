using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;

namespace RiskGame.API.Mappings
{
    public class AssetToModelReference : Profile
    {
        public AssetToModelReference()
        {
            CreateMap<Asset, ModelReference>();
        }
    }
    public class ModelReferenceToAsset : Profile
    {
        public ModelReferenceToAsset()
        {
            CreateMap<ModelReference, Asset>();
        }
    }
}
