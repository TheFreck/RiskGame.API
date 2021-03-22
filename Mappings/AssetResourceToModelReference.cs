using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;

namespace RiskGame.API.Mappings
{
    public class AssetResourceToModelReference : Profile
    {
        public AssetResourceToModelReference()
        {
            CreateMap<AssetResource, ModelReference>().ForMember(member => member.Id, config => config.MapFrom(og => Guid.Parse(og.AssetId)));
        }
    }
    public class ModelReferenceToAssetResource : Profile
    {
        public ModelReferenceToAssetResource()
        {
            CreateMap<ModelReference, AssetResource>().ForMember(member => member.AssetId, config => config.MapFrom(og => og.Id.ToString()));
        }
    }
}
