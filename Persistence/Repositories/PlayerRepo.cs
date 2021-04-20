using AutoMapper;
using MongoDB.Driver;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Persistence.Repositories
{
    public class PlayerRepo
    {
        private readonly IDatabaseSettings _dbSettings;
        private readonly IMongoCollection<PlayerResource> _players;
        private readonly IMapper _mapper;
        public PlayerRepo(IDatabaseSettings dbSettings, IMapper mapper)
        {
            _mapper = mapper;
            var client = new MongoClient(dbSettings.ConnectionString);
            var db = client.GetDatabase(dbSettings.DatabaseName);
            _dbSettings = dbSettings;
            _players = db.GetCollection<PlayerResource>(dbSettings.PlayerCollectionName);
        }
        // get one
        public PlayerResource GetOne(Guid playerId) => _players.AsQueryable().Where(p => p.PlayerId == playerId.ToString()).FirstOrDefault();
        // get many
        public List<PlayerResource> GetManySpecific(List<Guid> playerIds) => _players.AsQueryable().Where(p => playerIds.Contains(Guid.Parse(p.PlayerId))).ToList();
        public List<PlayerResource> GetMany(int quantity) => _players.AsQueryable().Take(quantity).ToList();
        // update one
        public Task<UpdateResult> UpdateOne(Guid playerId, UpdateDefinition<PlayerResource> update)
        {
            var filter = Builders<PlayerResource>.Filter.Eq("PlayerId",playerId.ToString());
            return _players.UpdateOneAsync(filter, update);
        }
        // update many
        public Task<UpdateResult> UpdateMany(List<Guid> players, UpdateDefinition<PlayerResource> updates)
        {
            var filter = Builders<PlayerResource>.Filter.AnyEq("PlayerId", players);
            return _players.UpdateManyAsync(filter, updates);
        }
        // delete one
        public Task<DeleteResult> DeleteOne(Guid playerId)
        {
            var filter = Builders<PlayerResource>.Filter.Eq("PlayerId", playerId.ToString());
            return _players.DeleteOneAsync(filter);
        }
        // delete many
        public Task<DeleteResult> DeleteMany(List<Guid> players)
        {
            var filter = Builders<PlayerResource>.Filter.AnyEq("PlayerId", players);
            return _players.DeleteManyAsync(filter);
        }
    }
}
