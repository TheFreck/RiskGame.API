using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB;
using MongoDB.Driver;
using RiskGame.API.Models.AssetFolder;

namespace RiskGame.API.Mappings
{
    public class IAsyncCursorToAsset : Profile
    {
        public IAsyncCursorToAsset()
        {
            CreateMap<IAsyncCursor<Asset>, Asset>();
        }
    }
}
