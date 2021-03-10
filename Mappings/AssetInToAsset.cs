using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models.AssetFolder;

namespace RiskGame.API.Mappings
{
    public class AssetInToAsset : Profile
    {
        public AssetInToAsset()
        {
            CreateMap<AssetIn, Asset>();
        }
    }
    public class AssetToAssetIn : Profile
    {
        public AssetToAssetIn()
        {
            CreateMap<Asset, AssetIn>();
        }
    }
}
