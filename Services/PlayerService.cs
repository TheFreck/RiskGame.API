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
        private readonly Random randy;

        public PlayerService(IShareRepo shareRepo, IAssetRepo assetRepo, IPlayerRepo playerRepo, IEconRepo econRepo, IPlayerLogic playerLogic, IMapper mapper, ITransactionService transactionService)
        {
            _shareRepo = shareRepo;
            _assetRepo = assetRepo;
            _playerRepo = playerRepo;
            _econRepo = econRepo;
            _playerLogic = playerLogic;
            _mapper = mapper;
            _transactionService = transactionService;
            randy = new Random();
        }

        public async void TradingStartStop(Guid gameId, bool isRunning)
        {
            var loopAction = await PlayerLoop(gameId);
            Console.WriteLine("player loop action: " + loopAction);
        }
        public Task<string> PlayerLoop(Guid gameId)
        {
            var allPlayers = _playerRepo.GetGamePlayers(gameId);
            var players = allPlayers.Where(p => p.Name != "HAUS");
            var haus = _playerRepo.GetHAUS().Where(h => h.GameId == gameId).FirstOrDefault();
            var assets = _assetRepo.GetGameAssets(gameId);
            var game = _econRepo.GetOne(gameId);
            do
            {
                var timer = new Stopwatch();
                timer.Start();
                foreach (var player in players)
                {
                    if(player.SkipsTilTurn != 0)
                    {
                        player.SkipsTilTurn--;
                        var result = _playerRepo.UpdateOne(player.PlayerId, Builders<PlayerResource>.Update.Set("SkipsTilTurn", player.SkipsTilTurn)).Result;
                        continue;
                    }
                    player.SkipsTilTurn = player.DecisionFrequency;
                    var updated = _playerRepo.UpdateOne(player.PlayerId, Builders<PlayerResource>.Update.Set("SkipsTilTurn", player.DecisionFrequency)).Result;
                    var shares = _shareRepo.GetMany(); // all shares
                    var playerShares = shares.Where(s => s.CurrentOwner.Id == player.PlayerId).ToArray(); // player shares
                    var tradeTicket = new TradeTicket();
                    tradeTicket.GameId = gameId;
                    tradeTicket.Asset = _mapper.Map<AssetResource,ModelReference>(assets[0]);
                    var decision = _playerLogic.PlayerTurn(player, playerShares, assets, game.History);
                    decision.Action = decision.Qty > 0 ? TurnTypes.Buy : decision.Qty < 0 ? TurnTypes.Sell : TurnTypes.Hold;
                    var total = decision.Asset;
                    tradeTicket.Shares = Math.Abs(decision.Qty);
                    var lastTrade = _assetRepo.GetGameAssets(gameId).Where(a => a.AssetId == tradeTicket.Asset.Id).FirstOrDefault();
                    var lastTradePrice = lastTrade.TradeHistory.OrderByDescending(t => t.Item1).FirstOrDefault().Item2;
                    var allShares = _shareRepo.GetMany().ToList();
                    var hausShares = allShares.Where(s => s.CurrentOwner.Id == haus.PlayerId).ToList();
                    decimal portionOfHausShares = (hausShares.Count() > 0 ? (decimal)(decision.Qty) / (decimal)(hausShares.Count()) : (decimal).001);
                    // ***************************************************************
                    // MODIFY THIS MODIFIER TO CREATE MORE DIRECTIONALITY IN THE PRICE
                    double priceDiff = (double)assets[0].CompanyAssetValuePerShare - (double)decision.Price;
                    var numerator = (decimal)(Math.Pow(4, 2 * priceDiff) - 1);
                    var denominator = (decimal)Math.Pow(4, priceDiff);
                    var addOn = (decimal)2.75 * (decimal)priceDiff;
                    decimal hausChange = numerator / denominator - addOn;
                    decimal scarcity = (decimal)tradeTicket.Shares / (decimal)hausShares.Count();
                    var lastTradeModifier = (1 + hausChange + scarcity);
                    // ***************************************************************

                    switch (decision.Action)
                    {
                        case TurnTypes.QuickSell:
                            tradeTicket.Buyer = GetHAUSRef(gameId);
                            tradeTicket.Seller = ResToRef(player);
                            tradeTicket.Cash = (decimal).99 * Math.Abs(lastTradePrice * lastTradeModifier * decision.Qty);
                            break;
                        case TurnTypes.Sell:
                            tradeTicket.Buyer = GetHAUSRef(gameId);
                            tradeTicket.Seller = ResToRef(player);
                            tradeTicket.Cash = (decimal).999 * Math.Abs(lastTradePrice * lastTradeModifier * decision.Qty);
                            break;
                        case TurnTypes.Hold:
                            continue;
                        case TurnTypes.Buy:
                            tradeTicket.Buyer = ResToRef(player);
                            tradeTicket.Seller = GetHAUSRef(gameId);
                            tradeTicket.Cash = (decimal)1.001 * Math.Abs(lastTradePrice * lastTradeModifier * decision.Qty);
                            break;
                        case TurnTypes.QuickBuy:
                            tradeTicket.Buyer = ResToRef(player);
                            tradeTicket.Seller = GetHAUSRef(gameId);
                            tradeTicket.Cash = (decimal)1.01 * Math.Abs(lastTradePrice * lastTradeModifier * decision.Qty);
                            break;
                    }
                    Console.WriteLine("****************************************************************************************************\n                         **************************************************");
                    Console.WriteLine("trade shares: " + tradeTicket.Shares);
                    Console.WriteLine("haus shares: " + hausShares.Count());
                    Console.WriteLine("last trade modifier: " + lastTradeModifier);
                    Console.WriteLine("price difference from last trade: " + (tradeTicket.Cash / tradeTicket.Shares - lastTradePrice));
                    Console.WriteLine("buyer: " + tradeTicket.Buyer.Name);
                    Console.WriteLine("seller: " + tradeTicket.Seller.Name);
                    Console.WriteLine("price: " + tradeTicket.Cash / tradeTicket.Shares);
                    Console.WriteLine("                         **************************************************                         \n****************************************************************************************************");
                    var traded = _transactionService.Transact(tradeTicket);
                    // add transaction to board
                    timer.Stop();
                }
                //Console.WriteLine("player: " + timer.ElapsedMilliseconds);
            } while (_econRepo.GetOne(gameId).isRunning);
            return Task.FromResult("player loop ended");
        }

        public PlayerResource GetHAUS(Guid gameId) => _playerRepo.GetHAUS().Where(p => p.GameId == gameId).FirstOrDefault();
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
            player.PlayerId = Guid.NewGuid();
            player.DecisionFrequency = player.DecisionFrequency < 5 ? randy.Next(2, 5) : player.DecisionFrequency;
            var playerResource = _mapper.Map<Player, PlayerResource>(player);
            _playerRepo.CreateOne(playerResource);
            return playerResource;
        }
        //
        // creates multiple new players
        public List<PlayerResource> CreateMany(List<Player> players)
        {
            players.ForEach(p => { p.PlayerId = Guid.NewGuid(); p.DecisionFrequency = randy.Next(2,5); });
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
        void TradingStartStop(Guid gameId, bool isRunning);
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
