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
    public class PlayerService : IPlayerService
    {
        private readonly IShareService _shareService;
        private readonly IMapper _mapper;
        private readonly IMongoCollection<PlayerResource> _players;
        private readonly Player HAUS;
        public PlayerService(IDatabaseSettings settings, IShareService shareService, IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            database.DropCollection(settings.PlayerCollectionName);
            _players = database.GetCollection<PlayerResource>(settings.PlayerCollectionName);
            _shareService = shareService;
            _mapper = mapper;

            HAUS = Create(new Player("HAUS",Guid.NewGuid()));
            var hausRes = _players.Find(h => h.PlayerId == HAUS.Id.ToString()).ToCursor();
            hausRes.ForEachAsync(o => HAUS.ObjectId = o.ObjectId);
            
        }
        //
        // Get the HAUS player
        public Player GetHAUS()
        {
            return HAUS;
        }
        public ModelReference GetHAUSRef()
        {
            return ToRef(HAUS);
        }
        //
        // Gets a list of all players in the DB
        public async Task<IAsyncCursor<PlayerResource>> GetAsync() =>
            await _players.FindAsync(player => true);
        //
        // Gets the player attached to the given id
        public async Task<IAsyncCursor<PlayerResource>> GetAsync(Guid id)
        {
            var anything = await _players.FindAsync(player => player.PlayerId == id.ToString());
            return anything;
        }
        //
        // Creates a new player from the JSON
        public Player Create(Player player)
        {
            _players.InsertOne(_mapper.Map<Player,PlayerResource>(player));
            return player;
        }
        //
        // Updates attributes of the Player
        // including updates to Cash and Portfolio
        public void Update(Guid id, PlayerResource playerIn)
        {
            var it = _players.ReplaceOne(player => player.PlayerId == id.ToString(), playerIn);
            var that = it;
        }
        //
        // Deletes the player in the db
        public void Remove(Player playerIn) =>
            _players.DeleteOne(player => player.PlayerId == playerIn.Id.ToString());
        //
        // Deletes the player in the db
        public void Remove(Guid id) =>
            _players.DeleteOne(player => player.PlayerId == id.ToString());
        //
        // Removes all assets from the player's portfolio
        //public async Task<string> EmptyPortfolio(Guid id)
        //{
        //    // get the player using the incoming guid
        //    var incoming = await _players.FindAsync<PlayerResource>(p => p.PlayerId == id.ToString());
        //    var player = new PlayerResource() { Name = "XXX"};
        //    await incoming.ForEachAsync(p => player = p);
        //    // if the guid did not find a player return out
        //    if (player.Name == "XXX") return "Player not found";
        //    // get the Shares from ModelReferences
        //    var guidList = new List<Guid>();
        //    player.Portfolio.ForEach(p => guidList.Add(p.Id));
        //    var incomingShares = await _shareService.GetAsync(guidList);
        //    var shares = new List<ShareResource>();
        //    // change Current Owner back to HAUS
        //    incomingShares.ForEach(s =>
        //    {
        //        shares.Add(new ShareResource() { CurrentOwner = ToRef(HAUS), ShareId = s.ShareId, ModelType = s.ModelType});
        //    });
        //    var returnRef = await _shareService.UpdateShares(shares);
        //    // replace the player's portfolio with an empty list
        //    player.Portfolio = new List<ModelReference>();

        //    var outcome = _players.FindOneAndReplaceAsync(p => p.PlayerId == player.PlayerId, player);
        //    if (player.Portfolio.Count != 0) return "Sumpin didn't work while clearing the portfolio";
        //    else return "Alls well that ends with an empty portfolio";
        //}
        //
        // Removes all cash from the player's wallet
        //public async Task<string> EmptyWallet(Guid id)
        //{
        //    var incoming = await _players.FindAsync<PlayerResource>(p => p.PlayerId == id.ToString());
        //    var player = new Player();
        //    await incoming.ForEachAsync(p => player = _mapper.Map<PlayerResource,Player>(p));
        //    if (player.Id == Guid.Empty) return "Player not found";

        //    var outcome = _players.FindOneAndReplaceAsync(p => p.PlayerId == player.Id.ToString(), _mapper.Map<Player,PlayerResource>(player)).Result;
        //    var oldCash = new List<Guid>();
        //    outcome.Wallet.ForEach(s => oldCash.Add(s.Id));
        //    var incomingShares = await _shareService.GetAsync(oldCash);
        //    var shares = new List<ShareResource>();
        //    var incomingHouse = _players.FindAsync(p => p.PlayerId == "D2157E01-5106-4607-B642-83622FDE4566").Result;
        //    var house = new PlayerResource();
        //    await incomingHouse.ForEachAsync(p => house = p);
        //    incomingShares.ForEach(s => { s.CurrentOwner = ResToRef(house); shares.Add(s); });

        //    var hem = _shareService.UpdateShares(shares);
        //    var update = Builders<PlayerResource>.Update.Set(p => p.Wallet, new List<ModelReference>());
        //    _players.UpdateOne(p => p.PlayerId == player.Id.ToString(), update);
        //    if (player.Wallet.Count != 0) return "Sumpin didn't work while clearing the cash";
        //    else return "Alls well that ends with empty cash";
        //}
        //public async Task<ModelReference> UpdateHaus(Player haus)
        //{
        //    try
        //    {
        //        var filter = Builders<PlayerResource>.Filter.Eq(s => s.PlayerId, haus.Id.ToString());
        //        var update = Builders<PlayerResource>.Update.Set(h => h.Portfolio, haus.Portfolio);
        //        await _players.UpdateOneAsync(filter, update);
        //        var hausRef = ToRef(HAUS);
        //        hausRef.Message = "Updated successfully";
        //        return hausRef;
        //    }
        //    catch (Exception e)
        //    {
        //        return new ModelReference { Message = $"Something went wrong while updating HAUS: {e.Message}" };
        //    }
        //}
        public ModelReference ToRef(Player player) =>
            _mapper.Map<Player,ModelReference>(player);
        public ModelReference ResToRef(PlayerResource player) =>
            _mapper.Map<PlayerResource, ModelReference>(player);
    }
    public interface IPlayerService
    {
        Player GetHAUS();
        ModelReference GetHAUSRef();
        Task<IAsyncCursor<PlayerResource>> GetAsync();
        Task<IAsyncCursor<PlayerResource>> GetAsync(Guid id);
        Player Create(Player player);
        void Update(Guid id, PlayerResource playerIn);
        void Remove(Player playerIn);
        void Remove(Guid id);
        //Task<string> EmptyPortfolio(Guid id);
        //Task<string> EmptyWallet(Guid id);
        //Task<ModelReference> UpdateHaus(Player haus);
        ModelReference ToRef(Player player);
        ModelReference ResToRef(PlayerResource player);
    }
}
