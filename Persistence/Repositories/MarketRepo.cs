using MongoDB.Driver;
using RiskGame.API.Models.MarketFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Persistence.Repositories
{
    public class MarketRepo : IMarketRepo
    {
        private readonly IDatabaseSettings _settings;
        private readonly IMongoCollection<MarketResource> _markets;
        public MarketRepo(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _markets = db.GetCollection<MarketResource>(settings.MarketCollectionName);
            _settings = settings;
        }
        // get one
        public MarketResource GetOne(Guid marketId) => _markets.AsQueryable().Where(p => p.MarketId == marketId).FirstOrDefault();
        // get many specific markets
        public IQueryable<MarketResource> GetManySpecific(List<Guid> playerIds) => _markets.AsQueryable().Where(p => playerIds.Contains(Guid.Parse(p.ObjectId)));
        // get many
        public IQueryable<MarketResource> GetMany() => _markets.AsQueryable();
        public void CreateOne(MarketResource market) => _markets.InsertOne(market);
        // update one
        public Task<UpdateResult> UpdateOne(Guid marketId, UpdateDefinition<MarketResource> update)
        {
            var filter = Builders<MarketResource>.Filter.Eq("PlayerId", marketId.ToString());
            return _markets.UpdateOneAsync(filter, update);
        }
        // update many
        public Task<UpdateResult> UpdateMany(List<Guid> markets, UpdateDefinition<MarketResource> updates)
        {
            var filter = Builders<MarketResource>.Filter.AnyEq("ObjectId", markets);
            return _markets.UpdateManyAsync(filter, updates);
        }
        // delete one
        public Task<DeleteResult> DeleteOne(Guid marketId)
        {
            var filter = Builders<MarketResource>.Filter.Eq("MarketId", marketId);
            return _markets.DeleteOneAsync(filter);
        }
        // delete many
        public Task<DeleteResult> DeleteMany(List<Guid> markets)
        {
            var filter = Builders<MarketResource>.Filter.AnyEq("ObjectId", markets);
            return _markets.DeleteManyAsync(filter);
        }
        // delete game markets
        public Task<DeleteResult> DeleteGameMarkets(Guid gameId) => _markets.DeleteManyAsync(Builders<MarketResource>.Filter.Eq("GameId",gameId));
    }
    public interface IMarketRepo
    {
        MarketResource GetOne(Guid playerId);
        IQueryable<MarketResource> GetManySpecific(List<Guid> playerIds);
        IQueryable<MarketResource> GetMany();
        void CreateOne(MarketResource market);
        Task<UpdateResult> UpdateOne(Guid playerId, UpdateDefinition<MarketResource> update);
        Task<UpdateResult> UpdateMany(List<Guid> players, UpdateDefinition<MarketResource> updates);
        Task<DeleteResult> DeleteOne(Guid playerId);
        Task<DeleteResult> DeleteMany(List<Guid> players);
        Task<DeleteResult> DeleteGameMarkets(Guid gameId);
    }
}
