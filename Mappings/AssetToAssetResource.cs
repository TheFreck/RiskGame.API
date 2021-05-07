using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;

namespace RiskGame.API.Mappings
{
    public class AssetToAssetResource : Profile
    {
        public AssetToAssetResource()
        {
            CreateMap<Asset, AssetResource>().ForMember(member => member.AssetId, config => config.MapFrom(og => og.AssetId));
        }
    }
    public class AssetResourceToAsset : Profile
    {
        public AssetResourceToAsset()
        {
            CreateMap<AssetResource, Asset>().ForMember(member => member.AssetId, config => config.MapFrom(og => og.AssetId));
        }
    }
    public class AssetResourceToAssetIn : Profile
    {
        public AssetResourceToAssetIn()
        {
            CreateMap<AssetResource, AssetIn>().ForMember(member => member.Id, config => config.MapFrom(og => og.AssetId));
        }
    }
}
