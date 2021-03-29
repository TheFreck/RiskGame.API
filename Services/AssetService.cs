using MongoDB.Driver;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;

namespace RiskGame.API.Services
{
    public class AssetService : IAssetService
    {
        private readonly IMongoCollection<AssetResource> _assets;
        private readonly IPlayerService _playerService;
        private readonly IMapper _mapper;
        private readonly AssetResource CASH;
        private readonly IDatabaseSettings dbSettings; // remove this when you remove Initialize
        public AssetService(IDatabaseSettings settings, IMapper mapper, IPlayerService playerService)
        {
            _playerService = playerService;
            dbSettings = settings;
            _mapper = mapper;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            database.DropCollection(settings.AssetCollectionName);
            database.DropCollection(settings.ShareCollectionName);
            _assets = database.GetCollection<AssetResource>(settings.AssetCollectionName);
            CASH = Create(new Asset(ModelTypes.Cash.ToString(), Guid.NewGuid()));
        }
        //
        // Drops the Asset Collection and recreates CASH
        public string Initialize()
        {
            try
            {
                _assets.Database.DropCollection(dbSettings.AssetCollectionName);
                _assets.Database.DropCollection(dbSettings.ShareCollectionName);
                var cash = Create(_mapper.Map<AssetResource,Asset>(CASH));
                return "Asset Tabula Rasa";
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }

        }
        public async Task<List<AssetResource>> GetAsync()
        {
            var foundAsssets = await _assets.FindAsync(a => a.AssetId != "");
            return foundAsssets.ToList();
        }
        public async Task<List<CompanyAsset>> GetCompanyAssets() => TakeCompanyAsset(await _assets.FindAsync(a => a.CompanyAsset != null));
        private List<CompanyAsset> TakeCompanyAsset(IAsyncCursor<AssetResource> foundAssets)
        {
            var companyAssets = new List<CompanyAsset>();
            foundAssets.ForEachAsync(a => companyAssets.Add(a.CompanyAsset));
            return companyAssets;
        }
        public AssetResource GetCash()
        {
            return CASH;
        }
        public async Task<IAsyncCursor<AssetResource>> GetSharesAsync(Guid id, ModelTypes type) 
        {
            var filterBase = Builders<AssetResource>.Filter;
            var filter = filterBase.Eq("ModelType", type) & filterBase.Eq("_assetId", id);
            return await _assets.FindAsync(filter);
        }
        public async Task<IAsyncCursor<AssetResource>> GetAsync(Guid id)
        {
            var asset = await _assets.FindAsync(asset => asset.AssetId == id.ToString());
            return asset;
        }
        public AssetResource Create(Asset asset)
        {
            var newAsset = _mapper.Map<Asset, AssetResource>(asset);
            _assets.InsertOne(newAsset);
            return newAsset;
        }
        public void Update(Guid id, Asset assetIn)
        {
            var assetRes = _mapper.Map<Asset, AssetResource>(assetIn);
            _assets.ReplaceOne(asset => asset.AssetId == id.ToString(), assetRes);
        }
        public void Remove(Asset assetIn)
        {
            var assetRes = _mapper.Map<Asset, AssetResource>(assetIn);         
            _assets.DeleteOne(asset => asset.AssetId.ToString() == assetRes.AssetId);
        }
        public void Remove(Guid id) =>
            _assets.DeleteOne(asset => asset.AssetId == id.ToString());
        public ModelReference ToRef(Asset asset) =>
            _mapper.Map<Asset,ModelReference>(asset);
    }
    public interface IAssetService
    {
        string Initialize();
        Task<List<AssetResource>> GetAsync();
        Task<List<CompanyAsset>> GetCompanyAssets();
        AssetResource GetCash(); Task<IAsyncCursor<AssetResource>> GetSharesAsync(Guid id, ModelTypes type);
        Task<IAsyncCursor<AssetResource>> GetAsync(Guid id);
        AssetResource Create(Asset asset);
        void Update(Guid id, Asset assetIn);
        void Remove(Asset assetIn);
        void Remove(Guid id);
        ModelReference ToRef(Asset asset);
    }
}