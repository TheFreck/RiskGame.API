using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;

namespace RiskGame.API.Models.PlayerFolder
{
    public class PlayerIn
    {
        public string Name { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public decimal Cash { get; set; }
        public double RiskTolerance { get; set; }
        public Sophistication Experience { get; set; }
    }
}
