using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.AssetFolder
{
    public class AssetResource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }
        public string AssetId { get; set; }
        public Guid GameId { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        public int SharesOutstanding { get; set; }
        public int LastBuyPrice { get; set; }
        public int LastSellPrice { get; set; }
        public string Message { get; set; }
        public readonly ModelTypes ModelType = ModelTypes.Asset;
        public CompanyAsset CompanyAsset { get; set; }
        public int Earnings { get; set; }
    }
}
