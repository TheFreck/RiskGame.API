using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Models.AssetFolder;

namespace RiskGame.API.Models.PlayerFolder
{
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId;
        public Guid Id { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        public List<Asset> Cash { get; set; }
        public int Risk { get; set; }
        public int Safety { get; set; }
        public List<ModelReference> Portfolio { get; set; }

    }
}
