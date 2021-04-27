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
        public Guid AssetId { get; set; }
        public Guid GameId { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        public int SharesOutstanding { get; set; }
        public double LastBuyPrice { get; set; }
        public double LastSellPrice { get; set; }
        public double MostRecentValue { get; set; }
        public int LastDividendPayout { get; set; }
        public DateTime LastDividendDate { get; set; }
        public string Message { get; set; }
        public ModelTypes ModelType { get; set; }
        public CompanyAsset CompanyAsset { get; set; }
        public int Debt { get; set; }
        public List<double> History { get; set; }
    }
}
