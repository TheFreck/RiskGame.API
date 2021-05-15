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
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        public decimal Cash { get; set; }
        public decimal NetWorth { get; set; }
        public double RiskTolerance { get; set; }
        public Sophistication Experience { get; set; }
        public Tuple<TradeType,decimal>[] TradeHistory { get; set; }
        public int DecisionFrequency { get; set; }
        //
        // model type is used to convert this class to a ModelReference
        // it is automatically set and cannot be overridden
        public readonly ModelTypes ModelType = ModelTypes.Player;
        public Player()
        {
            PlayerId = Guid.NewGuid();
        }
        public Player(Guid playerId)
        {
            PlayerId = playerId;
        }
        public Player(string name, Guid playerId, Guid gameId)
        {
            Name = name;
            PlayerId = playerId;
            GameId = gameId;
        }
        public Player(string name, Guid gameId)
        {
            Name = name;
            PlayerId = Guid.NewGuid();
            GameId = gameId;
        }
        public Player(PlayerIn playerIn)
        {
            Name = playerIn.Name;
            PlayerId = playerIn.PlayerId;
            GameId = playerIn.GameId;
            Cash = playerIn.Cash;
            RiskTolerance = playerIn.RiskTolerance;
            Experience = playerIn.Experience;
        }
    }
}
