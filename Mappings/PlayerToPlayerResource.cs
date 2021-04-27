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
            CreateMap<Player, PlayerResource>();
        }
    }
    public class PlayerResourceToPlayer : Profile
    {
        public PlayerResourceToPlayer()
        {
            CreateMap<PlayerResource, Player>();
        }
    }
}
