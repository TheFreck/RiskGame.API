using MongoDB.Driver;
using RiskGame.API.Models.SharesFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Persistence.Repositories
{
    public class ShareRepo : IShareRepo
    {
        private readonly IDatabaseSettings _settings;
        private readonly IMongoCollection<ShareResource> _shares;
        public ShareRepo(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _settings = settings;
            _shares = db.GetCollection<ShareResource>(settings.ShareCollectionName);
        }
        // get one
        public ShareResource GetOne(Guid shareId) => _shares.AsQueryable().Where(p => p.Id == shareId).FirstOrDefault();
        // get many
        public IQueryable<ShareResource> GetManySpecific(List<Guid> shareIds) => _shares.AsQueryable().Where(p => shareIds.Contains(p.Id));
        public IQueryable<ShareResource> GetMany() => _shares.AsQueryable();
        // create one
        public void CreateOne(ShareResource share) => _shares.InsertOne(share);
        public void CreateMany(IEnumerable<ShareResource> shares) => _shares.InsertMany(shares);
        // update one
        public Task<UpdateResult> UpdateOne(Guid shareId, UpdateDefinition<ShareResource> update)
        {
            var filter = Builders<ShareResource>.Filter.Eq("ShareId", shareId.ToString());
            return _shares.UpdateOneAsync(filter, update);
        }
        // update many
        public Task<UpdateResult> UpdateMany(IEnumerable<Guid> shareIds, UpdateDefinition<ShareResource> updates)
        {
            var filter = Builders<ShareResource>.Filter.In("ShareId", shareIds);
            var result = _shares.UpdateManyAsync(filter, updates);
            var trueResult = result.Result;
            return result;
        }
        // delete one
        public Task<DeleteResult> DeleteOne(Guid shareId)
        {
            var filter = Builders<ShareResource>.Filter.Eq("ShareId", shareId.ToString());
            return _shares.DeleteOneAsync(filter);
        }
        // delete many
        public Task<DeleteResult> DeleteMany(IEnumerable<Guid> shares)
        {
            var filter = Builders<ShareResource>.Filter.AnyEq("ShareId", shares);
            return _shares.DeleteManyAsync(filter);
        }
        // delete asset shares
        public Task<DeleteResult> DeleteAssetShares(Guid assetId)
        {
            var filter = Builders<ShareResource>.Filter.AnyEq("_assetId", assetId);
            return _shares.DeleteManyAsync(filter);
        }
    }
    public interface IShareRepo
    {
        ShareResource GetOne(Guid shareId);
        IQueryable<ShareResource> GetManySpecific(List<Guid> shareIds);
        IQueryable<ShareResource> GetMany();
        void CreateOne(ShareResource share);
        void CreateMany(IEnumerable<ShareResource> shares);
        Task<UpdateResult> UpdateOne(Guid shareId, UpdateDefinition<ShareResource> update);
        Task<UpdateResult> UpdateMany(IEnumerable<Guid> share, UpdateDefinition<ShareResource> updates);
        Task<DeleteResult> DeleteOne(Guid shareId);
        Task<DeleteResult> DeleteMany(IEnumerable<Guid> share);
        Task<DeleteResult> DeleteAssetShares(Guid assetId);
    }
}
