using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models;
using RiskGame.API.Models.PlayerFolder;

namespace RiskGame.API.Mappings
{
    public class PlayerToModelReference : Profile
    {
        public PlayerToModelReference()
        {
            CreateMap<Player, ModelReference >();
        }
    }
    public class ModelReferenceToPlayer : Profile
    {
        public ModelReferenceToPlayer()
        {
            CreateMap<ModelReference , Player>();
        }
    }
}
