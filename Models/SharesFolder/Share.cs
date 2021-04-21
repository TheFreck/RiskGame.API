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
using RiskGame.API.Entities.Enums;

namespace RiskGame.API.Models.SharesFolder
{
    public class Share
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId { get; set; }
        public Guid _assetId { get; set; }
        //
        // Share id is automatically set and can be overridden
        public Guid Id { get; set; }
        public string ShareId { get; set; }
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        //
        // the history will eventually preserve a record of each trade this share has been a part of
        public List<TradeRecord> History { get; set; }
        public ModelReference  CurrentOwner { get; set; }
        public int DividendOutstanding { get; set; }
        //
        // model type is used to convert this class to a ModelReference
        // it is automatically set and cannot be overridden
        public ModelTypes ModelType { get; set; }
        public Share(Guid assetId, string name, ModelReference  owner, ModelTypes type)
        {
            _assetId = assetId;
            Id = Guid.NewGuid();
            ShareId = Id.ToString();
            Name = name;
            CurrentOwner = owner;
            ModelType = type;
        }
        public Share(ModelTypes type)
        {
            Id = Guid.NewGuid();
            ShareId = Id.ToString();
            ModelType = type;
        }
        public Share()
        {
            Id = Guid.NewGuid();
            ShareId = Id.ToString();
        }
    }
}
