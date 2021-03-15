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
        //
        // Mongo id
        public string ObjectId;
        //
        // Player id is automatically set and can be overridden
        public Guid Id { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        //
        // a list of model references representing shares of the Asset cash
        public List<ModelReference> Wallet { get; set; }
        public int Cash { get; set; }
        public int Risk { get; set; }
        public int Safety { get; set; }
        //
        // a list of model references representing shares of the Assets being traded
        public List<ModelReference> Portfolio;
        //
        // model type is used to convert this class to a ModelReference
        // it is automatically set and cannot be overridden
        public readonly ModelTypes ModelType = ModelTypes.Player;
        public Player()
        {
            Id = Guid.NewGuid();
        }
    }
}
