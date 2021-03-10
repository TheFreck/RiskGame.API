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

namespace RiskGame.API.Models.SharesFolder
{
    public class Share
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId;
        public Guid _assetId;
        public Guid Id;
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name;
        public List<TradeTicket> History;
        public ModelReference CurrentOwner;
        public Share(Guid assetId, Guid id, string name)
        {
            _assetId = assetId;
            Id = id;
            Name = name;
        }
    }
}
