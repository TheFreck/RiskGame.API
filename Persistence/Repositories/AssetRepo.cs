using MongoDB.Driver;
using RiskGame.API.Models.AssetFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Persistence.Repositories
{
    public class AssetRepo : IAssetRepo
    {
        private readonly IDatabaseSettings _settings;
        private readonly IMongoCollection<AssetResource> _assets;
        public AssetRepo(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _settings = settings;
            _assets = db.GetCollection<AssetResource>(settings.AssetCollectionName);
        }
        // get one
        public AssetResource GetOne(Guid assetId) => _assets.AsQueryable().Where(p => p.AssetId == assetId.ToString()).FirstOrDefault();
        // get many
        public List<AssetResource> GetManySpecific(List<Guid> assetIds) => _assets.AsQueryable().Where(p => assetIds.Contains(Guid.Parse(p.AssetId))).ToList();
        public IQueryable<AssetResource> GetMany()
        {
            var sendit = _assets.AsQueryable();
            return sendit;
        }
        public AssetResource[] GetGameAssets(Guid gameId) => _assets.AsQueryable().Where(g => g.GameId == gameId).ToArray();
        public void CreateOne(AssetResource asset) => _assets.InsertOne(asset);
        // replace one
        public void ReplaceOne(Guid id, AssetResource asset) => _assets.ReplaceOne(asset => asset.AssetId == id.ToString(), asset);
        // update one
        public Task<UpdateResult> UpdateOne(Guid assetId, UpdateDefinition<AssetResource> update)
        {
            var filter = Builders<AssetResource>.Filter.Eq("AssetId", assetId.ToString());
            return _assets.UpdateOneAsync(filter, update);
        }
        // update many
        public Task<UpdateResult> UpdateMany(List<Guid> assetIds, UpdateDefinition<AssetResource> updates)
        {
            var filter = Builders<AssetResource>.Filter.In("AssetId", assetIds);
            var updatedAssets = _assets.UpdateManyAsync(filter, updates);
            var nupdatedNassets = updatedAssets.Result;
            return updatedAssets;
        }
        // delete one
        public Task<DeleteResult> DeleteOne(Guid assetId)
        {
            var filter = Builders<AssetResource>.Filter.Eq("AssetId", assetId.ToString());
            return _assets.DeleteOneAsync(filter);
        }
        // delete many
        public Task<DeleteResult> DeleteMany(List<Guid> assetIds)
        {
            var filter = Builders<AssetResource>.Filter.AnyEq("AssetId", assetIds);
            return _assets.DeleteManyAsync(filter);
        }
        public Task<DeleteResult> DeleteGameAssets(Guid gameId) => _assets.DeleteManyAsync(Builders<AssetResource>.Filter.Eq("GameId", gameId));
    }
    public interface IAssetRepo
    {
        AssetResource GetOne(Guid assetId);
        List<AssetResource> GetManySpecific(List<Guid> playeassetIdsrIds);
        IQueryable<AssetResource> GetMany();
        AssetResource[] GetGameAssets(Guid gameId);
        void CreateOne(AssetResource asset);
        void ReplaceOne(Guid id, AssetResource asset);
        Task<UpdateResult> UpdateOne(Guid assetId, UpdateDefinition<AssetResource> update);
        Task<UpdateResult> UpdateMany(List<Guid> assets, UpdateDefinition<AssetResource> updates);
        Task<DeleteResult> DeleteOne(Guid assetId);
        Task<DeleteResult> DeleteMany(List<Guid> assets);
        Task<DeleteResult> DeleteGameAssets(Guid gameId);
    }
}
