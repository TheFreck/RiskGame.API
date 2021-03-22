using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models;

namespace RiskGame.API.Mappings
{
    public class PlayerToPlayerResource : Profile
    {
        public PlayerToPlayerResource()
        {
            CreateMap<Player, PlayerResource>().ForMember(member => member.PlayerId, config => config.MapFrom(og => og.Id.ToString()));
        }
    }
    public class PlayerResourceToPlayer : Profile
    {
        public PlayerResourceToPlayer()
        {
            CreateMap<PlayerResource, Player>().ForMember(member => member.Id, config => config.MapFrom(og => Guid.Parse(og.PlayerId)));
        }
    }
}
