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
            CreateMap<ShareResource, ModelReference >().ForMember(member => member.Id, config => config.MapFrom(og => og.ShareId));
        }
    }
}
