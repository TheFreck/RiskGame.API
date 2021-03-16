using MongoDB.Driver;
using RiskGame.API.Models;
using RiskGame.API.Persistence;
using RiskGame.API.Models.PlayerFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models.SharesFolder;
using AutoMapper;

namespace RiskGame.API.Services
{
    public class PlayerService
    {
        private readonly ShareService _shareService;
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Player> _players;
        public readonly Player HAUS;
        public PlayerService(IDatabaseSettings settings, ShareService shareService, IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            client.DropDatabase(settings.DatabaseName);
            var database = client.GetDatabase(settings.DatabaseName);
            _players = database.GetCollection<Player>(settings.PlayerCollectionName);
            _shareService = shareService;
            _mapper = mapper;

            HAUS = new Player
            {
                Name = "HAUS",
                Id = Guid.NewGuid(),
                Portfolio = new List<ModelReference>(),
                Wallet = new List<ModelReference>(),
                Cash = 0
            };
            _players.InsertOne(HAUS);
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
            // get the player using the incoming guid
            var incoming = await _players.FindAsync<Player>(p => p.Id == id);
            var player = new Player() { Name = "XXX"};
            await incoming.ForEachAsync(p => player = p);
            // if the guid did not find a player return out
            if (player.Name == "XXX") return "Player not found";
            // get the Shares from ModelReferences
            var guidList = new List<Guid>();
            player.Portfolio.ForEach(p => guidList.Add(p.Id));
            var incomingShares = await _shareService.GetAsync(guidList);
            var shares = new List<Share>();
            // change Current Owner back to HAUS
            incomingShares.ForEach(s =>
            {
                shares.Add(new Share(ModelTypes.Share) { CurrentOwner = ToRef(HAUS), Id = s.Id });
            });
            var returnRef = await _shareService.UpdateShares(shares);
            // replace the player's portfolio with an empty list
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
            var outcome = _players.FindOneAndReplaceAsync(p => p.Id == player.Id, player).Result;
            var oldCash = new List<Guid>();
            outcome.Wallet.ForEach(s => oldCash.Add(s.Id));
            var incomingShares = await _shareService.GetAsync(oldCash);
            var shares = new List<Share>();
            var incomingHouse = _players.FindAsync(p => p.Id == Guid.Parse("D2157E01-5106-4607-B642-83622FDE4566")).Result;
            var house = new Player();
            await incomingHouse.ForEachAsync(p => house = p);
            incomingShares.ForEach(s => { s.CurrentOwner = this.ToRef(house); shares.Add(s); });

            var hem = _shareService.UpdateShares(shares);
            var update = Builders<Player>.Update.Set(p => p.Wallet, new List<ModelReference>());
            _players.UpdateOne(p => p.Id == player.Id, update);
            if (player.Wallet.Count != 0) return "Sumpin didn't work while clearing the cash";
            else return "Alls well that ends with empty cash";
        }
        public async Task<ModelReference> UpdateHaus(Player haus)
        {
            try
            {
                var filter = Builders<Player>.Filter.Eq(s => s.Id, haus.Id);
                var update = Builders<Player>.Update.Set(h => h.Portfolio, haus.Portfolio);
                await _players.UpdateOneAsync(filter, update);
                var hausRef = ToRef(HAUS);
                hausRef.Message = "Updated successfully";
                return hausRef;
            }
            catch (Exception e)
            {
                return new ModelReference { Message = $"Something went wrong while updating HAUS: {e.Message}" };
            }
        }
        public ModelReference ToRef(Player player) =>
            _mapper.Map<Player,ModelReference>(player);
    }
}
