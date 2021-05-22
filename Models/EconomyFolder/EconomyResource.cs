using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Entities;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.EconomyFolder
{
    public class EconomyResource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        //
        // Mongo id
        public string ObjectId { get; set; }
        [BsonElement("GameId")]
        [JsonProperty("GameId")]
        public Guid GameId { get; set; }
        public CompanyAsset[] Assets { get; set; }
        public List<Tuple<DateTime,MarketMetrics>> Markets { get; set; }
        public MarketMetricsHistory History { get; set; }
        public Player HAUS { get; set; }
        public bool isRunning { get; set; }
    }
}
