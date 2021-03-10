using MongoDB.Driver;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace RiskGame.API.Services
{
    public class AssetService
    {
        private readonly IMongoCollection<Asset> _assets;
        private readonly IMapper _mapper;
        private readonly ShareService _shareService;
        public AssetService(IDatabaseSettings settings, ShareService shareService, IMapper mapper)
        {
            _shareService = shareService;
            _mapper = mapper;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _assets = database.GetCollection<Asset>(settings.AssetCollectionName);
        }
        public List<Asset> Get() =>
            _assets.Find(asset => true).ToList();

        public Asset Get(Guid id) =>
            _assets.Find<Asset>(asset => asset.Id == id).FirstOrDefault();
        public Guid Create(Asset asset, int qty)
        {
            _assets.InsertOne(asset);
            _shareService.CreateShares(_mapper.Map<Asset,ModelReference>(asset), qty);
            return asset.Id;
        }
        public void Update(Guid id, Asset assetIn) =>
            _assets.ReplaceOne(asset => asset.Id == id, assetIn);
        public void Remove(Asset assetIn) =>
            _assets.DeleteOne(asset => asset.Id == assetIn.Id);
        public void Remove(Guid id) =>
            _assets.DeleteOne(asset => asset.Id == id);
    }
}