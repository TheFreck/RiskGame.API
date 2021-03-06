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
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        public decimal Cash { get; set; }
        public decimal NetWorth { get; set; }
        public double RiskTolerance { get; set; }
        public Sophistication Experience { get; set; }
        public Tuple<DateTime, TradeType, decimal>[] TradeHistory { get; set; }
        public int DecisionFrequency { get; set; }
        public int SkipsTilTurn { get; set; }
        //
        // model type is used to convert this class to a ModelReference
        // it is automatically set and cannot be overridden
        public readonly ModelTypes ModelType = ModelTypes.Player;
    }
}
