﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using RiskGame.API.Entities;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;

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
        [BsonElement("Name")]
        [JsonProperty("Name")]
        public string Name { get; set; }
        public int BookValue { get; set; }
        public int RateOfReturn { get; set; }
        public int SharesOutstanding { get; set; }
        public string Message { get; set; }
        //
        // model type is used to convert this class to a ModelReference
        // it is automatically set and cannot be overridden
        public readonly ModelTypes ModelType = ModelTypes.Asset;
        public Asset(string name, int rateOfReturn, int sharesOutstanding, Guid id)
        {
            Name = name;
            RateOfReturn = rateOfReturn;
            Id = id;
            SharesOutstanding = sharesOutstanding;
        }
        public Asset(string name, Guid id)
        {
            Name = name;
            Id = id;
            AssetId = Id.ToString();
        }
        public Asset() 
        {
            Id = Guid.NewGuid();
        }
    }
}
