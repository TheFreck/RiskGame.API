using MongoDB.Driver;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Persistence.Repositories
{
    public class PlayerRepo : IPlayerRepo
    {
        private readonly IDatabaseSettings _settings;
        private readonly IMongoCollection<PlayerResource> _players;
        public PlayerRepo(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _settings = settings;
            _players = db.GetCollection<PlayerResource>(settings.PlayerCollectionName);
        }
        //
        // get one
        public PlayerResource GetOne(Guid playerId) => _players.AsQueryable().Where(p => p.PlayerId == playerId).FirstOrDefault();
        //
        // get many
        public IQueryable<PlayerResource> GetManySpecific(List<Guid> playerIds) => _players.AsQueryable().Where(p => playerIds.Contains(p.PlayerId));
        //
        // get game players
        public IQueryable<PlayerResource> GetGamePlayers(Guid gameId) => _players.AsQueryable().Where(p => p.GameId == gameId);
        //
        // get haus
        public PlayerResource GetHAUS(/*Guid gameId*/) => _players.AsQueryable().Where(p => p.Name == "HAUS").FirstOrDefault();
        //
        // create one
        public void CreateOne(PlayerResource player) => _players.InsertOne(player);
        //
        // update one
        public void CreateMany(List<PlayerResource> players) => _players.InsertMany(players);
        public Task<UpdateResult> UpdateOne(Guid playerId, UpdateDefinition<PlayerResource> update)
        {
            var filter = Builders<PlayerResource>.Filter.Eq("PlayerId",playerId.ToString());
            return _players.UpdateOneAsync(filter, update);
        }
        //
        // update many
        public Task<UpdateResult> UpdateMany(List<Guid> players, UpdateDefinition<PlayerResource> updates)
        {
            var filter = Builders<PlayerResource>.Filter.AnyEq("PlayerId", players);
            return _players.UpdateManyAsync(filter, updates);
        }
        //
        // Replaces one with another
        public ReplaceOneResult Replace(FilterDefinition<PlayerResource> filter, PlayerResource player) => _players.ReplaceOne(filter, player);
        //
        // delete one
        public Task<DeleteResult> DeleteOne(Guid playerId)
        {
            var filter = Builders<PlayerResource>.Filter.Eq("PlayerId", playerId.ToString());
            return _players.DeleteOneAsync(filter);
        }
        //
        // delete many
        public Task<DeleteResult> DeleteMany(List<Guid> players)
        {
            var filter = Builders<PlayerResource>.Filter.AnyEq("PlayerId", players);
            return _players.DeleteManyAsync(filter);
        }
        //
        // delete all players in the game
        public Task<DeleteResult> DeleteGamePlayers(Guid gameId) => _players.DeleteManyAsync(Builders<PlayerResource>.Filter.Eq("GameId",gameId));
    }
    public interface IPlayerRepo
    {
        PlayerResource GetOne(Guid playerId);
        IQueryable<PlayerResource> GetManySpecific(List<Guid> playerIds);
        IQueryable<PlayerResource> GetGamePlayers(Guid gameId);
        PlayerResource GetHAUS(/*Guid gameId*/);
        void CreateOne(PlayerResource player);
        void CreateMany(List<PlayerResource> players);
        Task<UpdateResult> UpdateOne(Guid playerId, UpdateDefinition<PlayerResource> update);
        Task<UpdateResult> UpdateMany(List<Guid> players, UpdateDefinition<PlayerResource> updates);
        ReplaceOneResult Replace(FilterDefinition<PlayerResource> filter, PlayerResource player);
        Task<DeleteResult> DeleteOne(Guid playerId);
        Task<DeleteResult> DeleteMany(List<Guid> players);
        Task<DeleteResult> DeleteGamePlayers(Guid gameId);
    }
}
