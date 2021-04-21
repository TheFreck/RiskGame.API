using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Models;
using AutoMapper;

namespace RiskGame.API.Mappings
{
    public class ShareResourceToModelReference : Profile
    {
        public ShareResourceToModelReference()
        {
            CreateMap<ShareResource, ModelReference >().ForMember(member => member.Id, config => config.MapFrom(og => Guid.Parse(og.ShareId)));
        }
    }
    public class ModelReferenceToShareResource : Profile
    {
        public ModelReferenceToShareResource()
        {
            CreateMap<ModelReference , ShareResource>().ForMember(member => member.ShareId, config => config.MapFrom(og => og.Id.ToString()));
        }
    }
}
