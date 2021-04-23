using MongoDB.Driver;
using RiskGame.API.Models.EconomyFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Persistence.Repositories
{
    public class EconRepo : IEconRepo
    {
        private readonly IMongoCollection<EconomyResource> _econs;
        private readonly IDatabaseSettings _dbSettings;
        private readonly string _dbName;
        private readonly MongoClient _client;
        public EconRepo(IDatabaseSettings settings)
        {
            _dbSettings = settings;
            _client = new MongoClient(settings.ConnectionString);
            var db = _client.GetDatabase(settings.DatabaseName);
            _econs = db.GetCollection<EconomyResource>(settings.EconomyCollectionName);
            _dbName = settings.DatabaseName;
        }
        // get one
        public EconomyResource GetOne(Guid econId) => _econs.AsQueryable().Where(p => p.GameId == econId).FirstOrDefault();
        // get many
        public List<EconomyResource> GetManySpecific(List<Guid> econIds) => _econs.AsQueryable().Where(p => econIds.Contains(p.GameId)).ToList();
        // get all games
        public IQueryable<EconomyResource> GetAll() => _econs.AsQueryable();
        // create new game
        public void CreateOne(EconomyResource econ) => _econs.InsertOne(econ);
        // update one
        public Task<UpdateResult> UpdateOne(Guid econId, UpdateDefinition<EconomyResource> update)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", econId.ToString());
            return _econs.UpdateOneAsync(filter, update);
        }
        // update many
        public Task<UpdateResult> UpdateMany(List<Guid> econIds, UpdateDefinition<EconomyResource> updates)
        {
            var filter = Builders<EconomyResource>.Filter.AnyEq("GameId", econIds);
            return _econs.UpdateManyAsync(filter, updates);
        }
        // replace one
        public ReplaceOneResult ReplaceOne(FilterDefinition<EconomyResource> filter, EconomyResource econUpdate) => _econs.ReplaceOne(filter, econUpdate);
        // delete one
        public Task<DeleteResult> DeleteOne(Guid econId)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", econId.ToString());
            return _econs.DeleteOneAsync(filter);
        }
        // delete many
        public Task<DeleteResult> DeleteMany(List<Guid> econIds)
        {
            var filter = Builders<EconomyResource>.Filter.AnyEq("GameId", econIds);
            return _econs.DeleteManyAsync(filter);
        }
        // delete all
        public bool DeleteAll(string secretCode)
        {
            if (secretCode == _dbSettings.Destructo) _client.DropDatabase(_dbName);
            else return false;
            return true;
        }
    }
    public interface IEconRepo
    {
        EconomyResource GetOne(Guid econId);
        List<EconomyResource> GetManySpecific(List<Guid> econIds);
        IQueryable<EconomyResource> GetAll();
        void CreateOne(EconomyResource econ);
        Task<UpdateResult> UpdateOne(Guid econId, UpdateDefinition<EconomyResource> update);
        Task<UpdateResult> UpdateMany(List<Guid> econIds, UpdateDefinition<EconomyResource> updates);
        ReplaceOneResult ReplaceOne(FilterDefinition<EconomyResource> filter, EconomyResource econUpdate);
        Task<DeleteResult> DeleteOne(Guid econId);
        Task<DeleteResult> DeleteMany(List<Guid> econIds);
        bool DeleteAll(string secretCode);
    }
}
