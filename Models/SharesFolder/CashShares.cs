using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Entities;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace RiskGame.API.Models.SharesFolder
{
    public class CashShare
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }
        public Guid _assetId { get; set; }
        //
        // Share id is automatically set and can be overridden
        public Guid Id { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        //
        // the history will eventually preserve a record of each trade this share has been a part of
        public List<TradeTicket> History { get; set; }
        public ModelReference CurrentOwner { get; set; }
        //
        // model type is used to convert this class to a ModelReference
        // it is automatically set and cannot be overridden
        public readonly ModelTypes ModelType = ModelTypes.Cash;
        public CashShare(Guid assetId, string name, ModelReference owner)
        {
            _assetId = assetId;
            Id = Guid.NewGuid();
            Name = name;
            CurrentOwner = owner;
        }
        public CashShare()
        {
            Id = Guid.NewGuid();
        }
    }
}
