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
        }
        //
        // Drops the Asset Collection and recreates CASH
        //public string Initialize()
        //{
        //    try
        //    {
        //        _assets.Database.DropCollection(dbSettings.AssetCollectionName);
        //        _assets.Database.DropCollection(dbSettings.ShareCollectionName);
        //        var cash = Create(_mapper.Map<AssetResource,Asset>(CASH));
        //        return "Asset Tabula Rasa";
        //    }
        //    catch (Exception e)
        //    {
        //        return "Error: " + e.Message;
        //    }

        //}
        public async Task<List<AssetResource>> GetAsync()
        {
            var foundAsssets = await _assets.FindAsync(a => a.AssetId != "");
            return foundAsssets.ToList();
        }
        public CompanyAsset[] GetCompanyAssets(Guid gameId) => _assets.AsQueryable().Where(a => a.GameId == gameId).Select(a => a.CompanyAsset).ToArray();
        public List<CompanyAsset> TakeCompanyAsset(IAsyncCursor<AssetResource> foundAssets)
        {
            var companyAssets = new List<CompanyAsset>();
            foundAssets.ForEachAsync(a => {if(a.CompanyAsset != null) companyAssets.Add(a.CompanyAsset); });
            return companyAssets;
        }
        public async Task<IAsyncCursor<AssetResource>> GetCashAsync(Guid gameId)
        {
            var cash = await _assets.FindAsync(cash => cash.Name == ModelTypes.Cash.ToString() && cash.GameId == gameId);
            return cash;
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
        public AssetResource[] GetGameAssets(Guid id) => _assets.AsQueryable().Where(a => a.GameId == id).Where(a => a.CompanyAsset != null).ToArray();
        public AssetResource[] GetQueryableGameAssets(Guid gameId) => _assets.AsQueryable().Where(a => a.GameId == gameId).ToArray();
        public AssetResource Create(Asset asset)
        {
            var newAsset = _mapper.Map<Asset, AssetResource>(asset);
            _assets.InsertOne(newAsset);
            return newAsset;
        }
        public void Replace(Guid id, Asset assetIn)
        {
            var assetRes = _mapper.Map<Asset, AssetResource>(assetIn);
            _assets.ReplaceOne(asset => asset.AssetId == id.ToString(), assetRes);
        }
        public void Remove(Asset assetIn)
        {
            var assetRes = _mapper.Map<Asset, AssetResource>(assetIn);         
            _assets.DeleteOne(asset => asset.AssetId.ToString() == assetRes.AssetId);
        }
        public void RemoveFromGame(FilterDefinition<AssetResource> filter) => _assets.DeleteOne(filter);
        public DeleteResult RemoveAssetsFromGame(FilterDefinition<AssetResource> filter) => _assets.DeleteMany(filter);
        public ModelReference ToRef(Asset asset) => _mapper.Map<Asset,ModelReference>(asset);
        public ModelReference ResToRef(AssetResource asset) => _mapper.Map<AssetResource, ModelReference>(asset);
    }
    public interface IAssetService
    {
        //string Initialize();
        Task<List<AssetResource>> GetAsync();
        CompanyAsset[] GetCompanyAssets(Guid gameId);
        List<CompanyAsset> TakeCompanyAsset(IAsyncCursor<AssetResource> foundAssets);
        Task<IAsyncCursor<AssetResource>> GetCashAsync(Guid gameId);
        Task<IAsyncCursor<AssetResource>> GetSharesAsync(Guid id, ModelTypes type);
        Task<IAsyncCursor<AssetResource>> GetAsync(Guid id);
        AssetResource[] GetGameAssets(Guid id);
        AssetResource[] GetQueryableGameAssets(Guid gameId);
        AssetResource Create(Asset asset);
        void Replace(Guid id, Asset assetIn);
        void Remove(Asset assetIn);
        void RemoveFromGame(FilterDefinition<AssetResource> filter);
        DeleteResult RemoveAssetsFromGame(FilterDefinition<AssetResource> filter);
        ModelReference ToRef(Asset asset);
        ModelReference ResToRef(AssetResource asset);
    }
}