using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Entities;
using RiskGame.API.Models;

namespace RiskGame.API.Mappings
{
    public class ModelReferenceToId : Profile
    {
        public ModelReferenceToId()
        {
            CreateMap<ModelReference , SimpleId>();
        }
    }
}
