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
        // automatically set and can be overridden
        public Guid AssetId { get; set; }
        public Guid GameId { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        public int SharesOutstanding { get; set; }
        public List<Tuple<TradeType, decimal>> TradeHistory { get; set; }
        public decimal MostRecentValue { get; set; }
        public decimal LastDividendPayout { get; set; }
        public int PeriodsSinceDividend { get; set; }
        public string Message { get; set; }
        //
        // model type is used to convert this class to a ModelReference
        // it is automatically set and cannot be overridden
        public ModelTypes ModelType { get; }
        public CompanyAsset CompanyAsset { get; set; }
        public decimal CompanyAssetValuePerShare { get; set; }
        public int Debt { get; set; }
        public List<Tuple<DateTime, decimal>> CompanyHistory { get; set; }
        public Asset(string name, int sharesOutstanding, Guid assetId, Guid gameId, int debt)
        {
            Name = name;
            AssetId = assetId;
            SharesOutstanding = sharesOutstanding;
            GameId = gameId;
            Debt = debt;
            ModelType = ModelTypes.Asset;
            CompanyAsset = new CompanyAsset();
            CompanyHistory = new List<Tuple<DateTime, decimal>> { Tuple.Create(DateTime.Now, CompanyAsset.Value / sharesOutstanding) };
            TradeHistory = new List<Tuple<TradeType, decimal>>();
            CompanyAssetValuePerShare = (CompanyAsset.Value * Debt) / SharesOutstanding;
        }
        public Asset()
        {
            AssetId = Guid.NewGuid();
            Debt = new Random().Next(1, 10);
            ModelType = ModelTypes.Asset;
            CompanyAsset = new CompanyAsset();
        }
    }
}
