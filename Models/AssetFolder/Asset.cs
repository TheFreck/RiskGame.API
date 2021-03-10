using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Entities;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.AssetFolder
{
    public class Asset
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ObjectId;
        public Guid Id;
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name;
        public int BookValue;
        public int RateOfReturn;
        public int SharesOutstanding;
        public List<TradeTicket> History;
        public string Message;
        public Asset(string name, int bookValue, int rateOfReturn, int sharesOutstanding)
        {
            Name = name;
            RateOfReturn = rateOfReturn;
            Id = Id = Guid.NewGuid();
            SharesOutstanding = sharesOutstanding;
            BookValue = bookValue;
        }
        //public Asset() {}
    }
}
