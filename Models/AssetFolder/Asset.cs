using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Entities;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Entities.Enums;

namespace RiskGame.API.Models.AssetFolder
{
    public class Asset
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }
        //
        // Asset id is automatically set and can be overridden
        public  Guid Id { get; set; }
        public string AssetId { get; set; }
        public Guid GameId { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        public int SharesOutstanding { get; set; }
        public int LastBuyPrice { get; set; }
        public int LastSellPrice { get; set; }
        public double MostRecentValue { get; set; }
        public int LastDividendPayout { get; set; }
        public DateTime LastDividendDate { get; set; }
        public string Message { get; set; }
        //
        // model type is used to convert this class to a ModelReference
        // it is automatically set and cannot be overridden
        public readonly ModelTypes ModelType = ModelTypes.Asset;
        public CompanyAsset CompanyAsset { get; set; }
        public int Debt { get; set; }
        public List<double> History { get; set; }
        public Asset(string name, int sharesOutstanding, Guid id, Guid gameId)
        {
            Name = name;
            Id = id;
            AssetId = Id.ToString();
            SharesOutstanding = sharesOutstanding;
            GameId = gameId;
        }
        public Asset(string name, Guid id, Guid gameId)
        {
            Name = name;
            Id = id;
            AssetId = Id.ToString();
            GameId = gameId;
        }
        public Asset() 
        {
            Id = Guid.NewGuid();
            AssetId = Id.ToString();
        }
    }
}
