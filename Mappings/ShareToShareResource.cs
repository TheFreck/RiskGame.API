using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models.SharesFolder;
using AutoMapper;

namespace RiskGame.API.Mappings
{
    public class ShareToShareResource : Profile
    {
        public ShareToShareResource()
        {
            CreateMap<Share, ShareResource>().ForMember(member => member.ShareId, config => config.MapFrom(og => og.Id.ToString()));
        }
    }
    public class ShareResourceToShare: Profile
    {
        public ShareResourceToShare()
        {
            CreateMap<ShareResource, Share>().ForMember(member => member.Id, config => config.MapFrom(og => og.ShareId));
        }
    }
}
