using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Controllers;
using RiskGame.API.Entities;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Persistence;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models;
using AutoMapper;
using RiskGame.API.Models.SharesFolder;
using MongoDB.Driver;
using RiskGame.API.Logic;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Persistence.Repositories;
using RiskGame.API.Models.TransactionFolder;
using System.Threading;

namespace RiskGame.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionLogic _transactionLogic;
        private readonly IAssetRepo _assetRepo;
        private readonly IPlayerRepo _playerRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IMarketRepo _marketRepo;
        private readonly IEconRepo _econRepo;
        private readonly TransactionContext _transactionContext;
        private readonly IMapper _mapper;
        public TransactionService(ITransactionLogic transactionLogic, IAssetRepo assetRepo, IPlayerRepo playerRepo, IShareRepo shareRepo, IMarketRepo marketRepo, IEconRepo econRepo, TransactionContext transactionContext, IMapper mapper)
        {
            _mapper = mapper;
            _transactionLogic = transactionLogic;
            _assetRepo = assetRepo;
            _playerRepo = playerRepo;
            _shareRepo = shareRepo;
            _marketRepo = marketRepo;
            _econRepo = econRepo;
            _transactionContext = transactionContext;
        }
        public List<TransactionResource> GetTransactions(Guid gameId) => _transactionContext.GetAll(gameId);
        public async Task<TradeTicket> Transact(TradeTicket trade)
        {
            // early outs
            if(trade.Buyer == null & trade.Seller == null)
            {
                trade.Message = "If there's no buyer and no seller is there even a trade? Asking for an existential friend...";
                trade.SuccessfulTrade = false;
                return trade;
            } // OUT no buyer or seller
            if(trade.Buyer != null && trade.Cash == 0)
            {
                trade.Message = "No cash was included in the ticket";
                trade.SuccessfulTrade = false;
                return trade;
            } // OUT no cash offered
            if(trade.Seller != null && trade.Shares == 0)
            {
                trade.Message = "No shares were included in the ticket";
                trade.SuccessfulTrade = false;
                return trade;
            } // OUT no shares offered
            
            // get the schtuff you'll need
            var haus = _playerRepo.GetHAUS().Where(p => p.GameId == trade.GameId).FirstOrDefault();
            var buyer = trade.Buyer != null  && trade.Buyer.Name != haus.Name? _playerRepo.GetOne(trade.Buyer.Id) : haus;
            var seller = trade.Seller != null && trade.Seller.Name != haus.Name ? _playerRepo.GetOne(trade.Seller.Id) : haus;
            var sellerRef = _mapper.Map<PlayerResource,ModelReference>(seller);
            var buyerRef = _mapper.Map<PlayerResource, ModelReference>(buyer);
            var tradeAsset = _assetRepo.GetOne(trade.Asset.Id);
            
            // trade price
            decimal price = trade.Cash / trade.Shares;
            var tradeTuple = Tuple.Create(trade.Action, price);
            
            // trade history
            var buyerTradeHistory = buyer.TradeHistory.ToList();
            buyerTradeHistory.Add(tradeTuple);
            buyer.TradeHistory = buyerTradeHistory.ToArray();
            //
            var sellerTradeHistory = seller.TradeHistory.ToList();
            sellerTradeHistory.Add(tradeTuple);
            seller.TradeHistory = sellerTradeHistory.ToArray();
            //
            var assetTradeHistory = tradeAsset.TradeHistory;
            assetTradeHistory.Add(tradeTuple);
            tradeAsset.TradeHistory = assetTradeHistory;

            // process shares
            var assetShares = _shareRepo.GetMany();
            var tradeShares = assetShares.Where(s => s._assetId == trade.Asset.Id).Where(s => s.CurrentOwner.Id == seller.PlayerId).Take(trade.Shares).ToArray();

            // update buyer and seller networth
            buyer.NetWorth = tradeTuple.Item2 * tradeShares.Length + buyer.Cash;
            seller.NetWorth = tradeTuple.Item2 * tradeShares.Length + seller.Cash;
            try
            {
                // Transfer cash
                seller.Cash += trade.Cash;
                buyer.Cash -= trade.Cash;

                // Transfer shares
                var transferredShares = _transactionLogic.TransferShares(buyer, tradeShares, trade.Shares);
                var shrs = await _shareRepo.UpdateMany(transferredShares.Select(s => s.ShareId).ToList(),Builders<ShareResource>.Update.Set("CurrentOwner", buyerRef));

                // Update asset
                var assetUpdateBase = Builders<AssetResource>.Update;
                var assetUpdateResult = _assetRepo.UpdateOne(tradeAsset.AssetId, assetUpdateBase.Set("TradeHistory", assetTradeHistory));
                
                // update buyer and seller
                var playerUpdateBase = Builders<PlayerResource>.Update;
                var buyerUpdate = playerUpdateBase
                                    .Set("Cash", buyer.Cash)
                                    .Set("TradeHistory", buyerTradeHistory.ToArray())
                                    .Set("NetWorth",buyer.NetWorth);
                var sellerUpdate = playerUpdateBase
                                    .Set("Cash", seller.Cash)
                                    .Set("TradeHistory", sellerTradeHistory.ToArray())
                                    .Set("NetWorth", seller.NetWorth);
                var buyerResult = await _playerRepo.UpdateOne(buyer.PlayerId, buyerUpdate);
                var sellerResult = await _playerRepo.UpdateOne(seller.PlayerId, sellerUpdate);

                // submit trade
                trade.TradeId = Guid.NewGuid();
                _transactionContext.AddTrade(_mapper.Map<TradeTicket, TransactionResource>(trade));
                
                // complete the trade ticket
                trade.SuccessfulTrade = true;
                return trade;
            }
            catch (Exception e)
            {
                trade.Message = $"Something went wrong while trying to perform this trade: {e.Message}";
                trade.TradeTime = DateTime.Now;
                trade.SuccessfulTrade = false;
                return trade;
            }
        }
        public void InsertTrade(TransactionResource trade) => _transactionContext.AddTrade(trade);
    }
    public interface ITransactionService
    {
        List<TransactionResource> GetTransactions(Guid gameId);
        Task<TradeTicket> Transact(TradeTicket trade);
        void InsertTrade(TransactionResource trade);
    }
}