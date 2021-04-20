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
using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Logic;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Entities;

namespace RiskGame.API.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IShareService _shareService;
        private readonly IMapper _mapper;
        private readonly IMongoCollection<PlayerResource> _players;
        private readonly IEconService _econService;
        private readonly IAssetService _assetService;
        private readonly ITransactionService _transactionService;
        private readonly IPlayerLogic _playerLogic;
        private readonly IDatabaseSettings dbSettings; // remove this when you remove Initialize
        public PlayerService(IDatabaseSettings settings, IShareService shareService, IEconService econService, IAssetService assetService, ITransactionService transactionService, IPlayerLogic playerLogic, IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            database.DropCollection(settings.PlayerCollectionName);
            dbSettings = settings;
            _players = database.GetCollection<PlayerResource>(settings.PlayerCollectionName);
            _shareService = shareService;
            _econService = econService;
            _playerLogic = playerLogic;
            _assetService = assetService;
            _transactionService = transactionService;
            _mapper = mapper;
        }
        public Task<string> PlayerLoop(Guid gameId)
        {
            var players = _players.AsQueryable().Where(p => p.GameId == gameId).ToList();
            var assets = _assetService.GetGameAssets(gameId);
            var game = _econService.GetGame(gameId);
            var keepGoing = _econService.IsRunning(gameId);
            do
            {
                foreach(var player in players)
                {
                    var tradeTicket = new TradeTicket();
                    tradeTicket.GameId = gameId;
                    tradeTicket.Asset = _assetService.ResToRef(assets[0]);
                    var outcome = _playerLogic.PlayerTurn(player, assets, game.History);
                    var total = (TurnTypes)((int)Math.Floor(new int[] {(int)outcome.Allocation, (int)outcome.Asset}.Average()));
                    tradeTicket.Shares = outcome.Qty;

                    switch (total)
                    {
                        case TurnTypes.QuickSell:
                            tradeTicket.Buyer = GetHAUSRef(gameId);
                            tradeTicket.Seller = ResToRef(player);
                            tradeTicket.Cash = (int)Math.Floor(.95*(outcome.Value * outcome.Qty));
                            break;
                        case TurnTypes.Sell:
                            tradeTicket.Buyer = GetHAUSRef(gameId);
                            tradeTicket.Seller = ResToRef(player);
                            tradeTicket.Cash = (int)Math.Ceiling(.99*(outcome.Value * outcome.Qty));
                            break;
                        case TurnTypes.Hold:
                            continue;
                        case TurnTypes.Buy:
                            tradeTicket.Buyer = ResToRef(player);
                            tradeTicket.Seller = GetHAUSRef(gameId);
                            tradeTicket.Cash = (int)Math.Floor(1.01*(outcome.Value * outcome.Qty));
                            break;
                        case TurnTypes.QuickBuy:
                            tradeTicket.Buyer = ResToRef(player);
                            tradeTicket.Seller = GetHAUSRef(gameId);
                            tradeTicket.Cash = (int)Math.Ceiling(1.05 * (outcome.Value * outcome.Qty));
                            break;
                    }
                    var traded = _transactionService.Transact(tradeTicket);
                    // add transaction to board
                }
            } while (keepGoing);
            return Task.FromResult("player loop ended");
        }
        public string MassDestruction()
        {
            _players.Database.Client.DropDatabase(dbSettings.DatabaseName);
            return "it is done";
        }
        public async Task<Player> GetHAUS(Guid gameId)
        {
            var filterBase = Builders<PlayerResource>.Filter;
            var filter = filterBase.Eq("GameId", gameId) & filterBase.Eq("Name","HAUS");
            var incoming = _players.FindAsync(filter).Result;
            var haus = new PlayerResource();
            await incoming.ForEachAsync(p => haus = p);
            return _mapper.Map<PlayerResource,Player>(haus);
        }
        public ModelReference GetHAUSRef(Guid gameId)
        {
            return ToRef(GetHAUS(gameId).Result);
        }
        //
        // Gets a list of all players in the DB
        public async Task<IAsyncCursor<PlayerResource>> GetAsync() => await _players.FindAsync(player => true);
        //
        // Gets the player attached to the given id
        public PlayerResource GetPlayerResource(Guid playerId) => _players.AsQueryable().Where(p => p.PlayerId == playerId.ToString()).FirstOrDefault();
        public async Task<IAsyncCursor<PlayerResource>> GetPlayerAsync(Guid playerId) => await _players.FindAsync(player => player.PlayerId == playerId.ToString());
        //
        // Remove all players from a game
        public string RemovePlayersFromGame(Guid gameId)
        {
            var filter = Builders<PlayerResource>.Filter.Eq("GameId", gameId);
            _players.DeleteMany(filter);
            return "they were 'removed' peacefully in their sleep";
        }
        //
        // Creates a new player from the JSON
        public PlayerResource Create(Player player)
        {
            var newPlayer = _mapper.Map<Player, PlayerResource>(player);
            _players.InsertOne(newPlayer);
            return newPlayer;
        }
        //
        // Updates attributes of the Player
        // including updates to Cash and Portfolio
        public void Update(Guid id, PlayerResource playerIn) => _players.ReplaceOne(player => player.PlayerId == id.ToString(), playerIn);
        //
        // Deletes the player in the db
        public void Remove(Player playerIn) => _players.DeleteOne(player => player.PlayerId == playerIn.Id.ToString());
        //
        // Deletes the player in the db
        public void Remove(Guid id) => _players.DeleteOne(player => player.PlayerId == id.ToString());
        public ModelReference ToRef(Player player) => _mapper.Map<Player,ModelReference>(player);
        public ModelReference ResToRef(PlayerResource player) => _mapper.Map<PlayerResource, ModelReference>(player);
    }
    public interface IPlayerService
    {
        string MassDestruction();
        Task<Player> GetHAUS(Guid gameId);
        ModelReference GetHAUSRef(Guid gameId);
        Task<IAsyncCursor<PlayerResource>> GetAsync();
        PlayerResource GetPlayerResource(Guid playerId);
        Task<IAsyncCursor<PlayerResource>> GetPlayerAsync(Guid id);
        string RemovePlayersFromGame(Guid gameId);
        PlayerResource Create(Player player);
        void Update(Guid id, PlayerResource playerIn);
        void Remove(Player playerIn);
        void Remove(Guid id);
        ModelReference ToRef(Player player);
        ModelReference ResToRef(PlayerResource player);
    }
}
