using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.PlayerFolder
{
    public class PlayerResource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        //
        // Mongo id
        public string ObjectId { get; set; }
        public string PlayerId { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        public int Cash { get; set; }
        public int RiskTolerance { get; set; }
        //
        // model type is used to convert this class to a ModelReference
        // it is automatically set and cannot be overridden
        public readonly ModelTypes ModelType = ModelTypes.Player;
    }
}
