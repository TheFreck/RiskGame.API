using MongoDB.Driver;
using RiskGame.API.Models;
using RiskGame.API.Persistence;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Services
{
    public class PlayerService
    {
        private readonly IMongoCollection<Player> _players;
        public PlayerService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _players = database.GetCollection<Player>(settings.PlayerCollectionName);
        }
        public List<Player> Get() =>
            _players.Find(player => true).ToList();

        public Player Get(Guid id) =>
            _players.Find<Player>(player => player.Id == id).FirstOrDefault();
        public Player Create(Player player)
        {
            _players.InsertOne(player);
            return player;
        }
        public void Update(Guid id, Player playerIn) =>
            _players.ReplaceOne(player => player.Id == id, playerIn);
        public void Remove(Player playerIn) =>
            _players.DeleteOne(player => player.Id == playerIn.Id);
        public void Remove(Guid id) =>
            _players.DeleteOne(player => player.Id == id);
    }
}
