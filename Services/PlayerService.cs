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
        //
        // Gets a list of all players in the DB
        public async Task<IAsyncCursor<Player>> GetAsync() =>
            await _players.FindAsync(player => true);
        //
        // Gets the player attached to the given id
        public async Task<IAsyncCursor<Player>> GetAsync(Guid id) =>
            await _players.FindAsync(player => player.Id == id);
        //
        // Creates a new player from the JSON
        public Player Create(Player player)
        {
            _players.InsertOne(player);
            return player;
        }
        //
        // Updates attributes of the Player
        // including updates to Cash and Portfolio
        public void Update(Guid id, Player playerIn) =>
            _players.ReplaceOne(player => player.Id == id, playerIn);
        //
        // Deletes the player in the db
        public void Remove(Player playerIn) =>
            _players.DeleteOne(player => player.Id == playerIn.Id);
        //
        // Deletes the player in the db
        public void Remove(Guid id) =>
            _players.DeleteOne(player => player.Id == id);
        //
        // Removes all assets from the player's portfolio
        public async Task<string> EmptyPortfolio(Guid id)
        {
            var incoming = await _players.FindAsync<Player>(p => p.Id == id);
            var player = new Player();
            await incoming.ForEachAsync(p => player = p);
            if (player.Id == Guid.Empty) return "Player not found";
            player.Portfolio = new List<ModelReference>();
            var outcome = _players.FindOneAndReplaceAsync(p => p.Id == player.Id, player);
            if (player.Portfolio.Count != 0) return "Sumpin didn't work while clearing the portfolio";
            else return "Alls well that ends with an empty portfolio";
        }
        //
        // Removes all cash from the player's wallet
        public async Task<string> EmptyWallet(Guid id)
        {
            var incoming = await _players.FindAsync<Player>(p => p.Id == id);
            var player = new Player();
            await incoming.ForEachAsync(p => player = p);
            if (player.Id == Guid.Empty) return "Player not found";
            player.Wallet = new List<ModelReference>();
            var outcome = _players.FindOneAndReplaceAsync(p => p.Id == player.Id, player);
            if (player.Wallet.Count != 0) return "Sumpin didn't work while clearing the cash";
            else return "Alls well that ends with an empty cash";
        }
    }
}
