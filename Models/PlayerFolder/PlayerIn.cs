using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RiskGame.API.Models.AssetFolder;

namespace RiskGame.API.Models.PlayerFolder
{
    public class PlayerIn
    {
        public string Name { get; set; }
        public List<ModelReference> Wallet { get; set; }
        public int Cash { get; set; }
        public int? Risk { get; set; }
        public int? Safety { get; set; }
        public List<ModelReference> Portfolio { get; set; }
    }
}
