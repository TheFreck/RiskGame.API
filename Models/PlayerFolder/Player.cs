using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Services;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;

namespace RiskGame.API.Models.PlayerFolder
{
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        //
        // Mongo id
        public string ObjectId { get; set; }
        //
        // Player id is automatically set and can be overridden
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
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
        public Player()
        {
            Id = Guid.NewGuid();
            PlayerId = Id.ToString();
        }
        public Player(Guid id)
        {
            Id = id;
            PlayerId = Id.ToString();
        }
        public Player(string name, Guid id, Guid gameId)
        {
            Name = name;
            Id = id;
            PlayerId = id.ToString();
            GameId = gameId;
        }
    }
}
