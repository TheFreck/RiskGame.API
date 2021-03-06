using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models;
using AutoMapper;

namespace RiskGame.API.Mappings
{
    public class PlayerResourseToModelReference : Profile
    {
        public PlayerResourseToModelReference()
        {
            CreateMap<PlayerResource, ModelReference>().ForMember(member => member.Id, config => config.MapFrom(og => og.PlayerId));
        }
    }
}
