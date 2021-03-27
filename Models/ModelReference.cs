using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models
{
    public class ModelReference
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public ModelTypes ModelType { get; set; }
        public string Message { get; set; }
    }
}
