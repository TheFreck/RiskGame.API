using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Models;

namespace RiskGame.API.Mappings
{
    public class ShareToModelReference : Profile
    {
        public ShareToModelReference()
        {
            CreateMap<Share, ModelReference >();
        }
    }
    public class ModelReferenceToShare : Profile
    {
        public ModelReferenceToShare()
        {
            CreateMap<ModelReference , Share>();
        }
    }
}
