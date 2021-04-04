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
        public CompanyAsset[] Assets;
        public List<MarketMetrics> Markets;
        public Player HAUS { get; set; }
        public Asset CASH { get; set; }
        public Economy()
        {
            isRunning = false;
            Markets = new List<MarketMetrics> {
                new MarketMetrics
                {
                    Red = 1,
                    Orange = 1,
                    Yellow = 1,
                    Green = 1,
                    Blue = 1,
                    Violet = 1,
                    RedDirection = Direction.Up,
                    OrangeDirection = Direction.Up,
                    YellowDirection = Direction.Up,
                    GreenDirection = Direction.Up,
                    BlueDirection = Direction.Up,
                    VioletDirection = Direction.Up
                }
            };
        }

        // generate economic metrics
        // grow Company Assets
        // process players' turns
    }
}
