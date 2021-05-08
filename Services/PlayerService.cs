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
using RiskGame.API.Persistence.Repositories;
using RiskGame.API.Models.AssetFolder;
using System.Diagnostics;
using RiskGame.API.Models.TransactionFolder;

namespace RiskGame.API.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IMapper _mapper;
        private readonly IPlayerRepo _playerRepo;
        private readonly IAssetRepo _assetRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IEconRepo _econRepo;
        private readonly ITransactionService _transactionService;

        private readonly IPlayerLogic _playerLogic;
        public PlayerService(IShareRepo shareRepo, IAssetRepo assetRepo, IPlayerRepo playerRepo, IEconRepo econRepo, IPlayerLogic playerLogic, IMapper mapper, ITransactionService transactionService)
        {
            _shareRepo = shareRepo;
            _assetRepo = assetRepo;
            _playerRepo = playerRepo;
            _econRepo = econRepo;
            _playerLogic = playerLogic;
            _mapper = mapper;
            _transactionService = transactionService;
        }

        public async void TradingStartStop(Guid gameId)
        {
            var loopAction = await PlayerLoop(gameId);
            Console.WriteLine("player loop action: " + loopAction);
        }
        public Task<string> PlayerLoop(Guid gameId)
        {
            var players = _playerRepo.GetGamePlayers(gameId);
            var assets = _assetRepo.GetGameAssets(gameId);
            var game = _econRepo.GetOne(gameId);
            do
            {
                var timer = new Stopwatch();
                timer.Start();
                foreach (var player in players)
                {
                    var shares = _shareRepo.GetMany();
                    var theShares = shares.Where(s => s.CurrentOwner.Id == player.PlayerId).ToArray();
                    var tradeTicket = new TradeTicket();
                    tradeTicket.GameId = gameId;
                    tradeTicket.Asset = _mapper.Map<AssetResource,ModelReference>(assets[0]);
                    var outcome = _playerLogic.PlayerTurn(player, _mapper.Map<ShareResource[],Share[]>(theShares), assets, game.History);
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
                    timer.Stop();
                }
                Console.WriteLine("player: " + timer.ElapsedMilliseconds);
            } while (_econRepo.GetOne(gameId).isRunning);
            return Task.FromResult("player loop ended");
        }

        public PlayerResource GetHAUS(Guid gameId) => _playerRepo.GetHAUS(/*gameId*/);
        public ModelReference GetHAUSRef(Guid gameId) => ResToRef(GetHAUS(gameId));
        //
        // Gets the player attached to the given id
        public PlayerResource GetPlayer(Guid playerId) => _playerRepo.GetOne(playerId);
        //
        // Remove all players from a game
        public DeleteResult RemovePlayersFromGame(Guid gameId) => _playerRepo.DeleteMany(_playerRepo.GetGamePlayers(gameId).Select(p => p.GameId).ToList()).Result;
        //
        // Creates a new player from the JSON
        public PlayerResource CreateOne(Player player)
        {
            var playerResource = _mapper.Map<Player, PlayerResource>(player);
            var playerCash = _shareRepo.GetMany().Where(s => s.ModelType == ModelTypes.Cash);
            var update = Builders<ShareResource>.Update.Set("CurrentOwner", ToRef(player));
            var updated = _shareRepo.UpdateMany(playerCash.Select(c => c.ShareId),update).Result;
            _playerRepo.CreateOne(playerResource);
            return playerResource;
        }
        //
        // creates multiple new players
        public List<PlayerResource> CreateMany(List<Player> players)
        {
            var newPlayers = _mapper.Map<List<Player>, List<PlayerResource>>(players);
            _playerRepo.CreateMany(newPlayers);
            return newPlayers;
        }
        //
        // Updates attributes of the Player
        // including updates to Cash and Portfolio
        public UpdateResult Update(Guid id, UpdateDefinition<PlayerResource> update) => _playerRepo.UpdateOne(id, update).Result;
        //
        // replaces a player with the input
        public ReplaceOneResult Replace(FilterDefinition<PlayerResource> filter, PlayerResource player) => _playerRepo.Replace(filter, player);
        //
        // Deletes the player in the db
        public DeleteResult Remove(Guid playerId) => _playerRepo.DeleteOne(playerId).Result;
        public ModelReference ToRef(Player player) => _mapper.Map<Player, ModelReference>(player);
        public ModelReference ResToRef(PlayerResource player) => _mapper.Map<PlayerResource, ModelReference>(player);
    }
    public interface IPlayerService
    {
        void TradingStartStop(Guid gameId);
        Task<string> PlayerLoop(Guid gameId);
        PlayerResource GetHAUS(Guid gameId);
        ModelReference GetHAUSRef(Guid gameId);
        PlayerResource GetPlayer(Guid playerId);
        DeleteResult RemovePlayersFromGame(Guid gameId);
        PlayerResource CreateOne(Player player);
        List<PlayerResource> CreateMany(List<Player> players);
        UpdateResult Update(Guid id, UpdateDefinition<PlayerResource> update);
        ReplaceOneResult Replace(FilterDefinition<PlayerResource> filter, PlayerResource player);
        DeleteResult Remove(Guid id);
        ModelReference ToRef(Player player);
        ModelReference ResToRef(PlayerResource player);
    }
}
