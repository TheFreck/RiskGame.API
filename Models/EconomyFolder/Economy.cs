using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.EconomyFolder
{
    public class Economy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        //
        // Mongo id
        public string ObjectId { get; set; }
        [BsonElement("GameId")]
        [JsonProperty("GameId")]
        public Guid GameId = Guid.NewGuid();
        public bool isRunning { get; set; }
        public int PixelCount { get; set; }
        public int Trendiness { get; set; }
        public CompanyAsset[] Assets { get; set; }
        public List<MarketMetrics> Markets = new List<MarketMetrics>();
        public MarketMetricsHistory History = new MarketMetricsHistory();
        public Player HAUS { get; set; }
        public Asset CASH { get; set; }
    }
}
