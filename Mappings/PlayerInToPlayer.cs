using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models;
using RiskGame.API.Models.PlayerFolder;

namespace RiskGame.API.Mappings
{
    public class PlayerInToPlayer : Profile
    {
        public PlayerInToPlayer()
        {
            CreateMap<PlayerIn, Player>();
        }
    }
    public class PlayerToPlayerIn : Profile
    {
        public PlayerToPlayerIn()
        {
            CreateMap<Player, PlayerIn>();
        }
    }
    public class PlayerInToModelReference : Profile
    {
        public PlayerInToModelReference()
        {
            CreateMap<PlayerIn, ModelReference>().ForMember(member => member.Id, config => config.MapFrom(og => og.Id));
        }
    }
}
