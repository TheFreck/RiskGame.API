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
            // get Haus
            var haus = _playerRepo.GetHAUS(/*trade.GameId*/);
            // get Buyer
            var buyer = trade.Buyer != null ? _playerRepo.GetOne(trade.Buyer.Id) : haus;
            buyer.Cash -= trade.Cash;

            // get Seller
            var seller = trade.Seller != null ? _playerRepo.GetOne(trade.Seller.Id) : haus;
            seller.Cash += trade.Cash;
            // get References
            var sellerRef = _mapper.Map<PlayerResource,ModelReference>(seller);
            var buyerRef = _mapper.Map<PlayerResource, ModelReference>(buyer);
            // get cash and shares
            var tradeAsset = _assetRepo.GetOne(trade.Asset.Id);
            var tradeShares = _shareRepo.GetMany().Where(s => s.CurrentOwner.Id == seller.PlayerId).Where(s => s._assetId == trade.Asset.Id).Take(trade.Shares).ToArray();
            try
            {
                // Transfer ownership of shares
                var transferredShares = _transactionLogic.TransferShares(buyer, tradeShares, trade.Shares);
                var shrs = await _shareRepo.UpdateMany(transferredShares.Select(s => s.ShareId).ToList(),Builders<ShareResource>.Update.Set("CurrentOwner", buyerRef));
                var updateBuilder = Builders<PlayerResource>.Update;
                await _playerRepo.UpdateOne(buyer.PlayerId, updateBuilder.Set("Cash", buyer.Cash));
                await _playerRepo.UpdateOne(seller.PlayerId, updateBuilder.Set("Cash", seller.Cash));
                // complete the trade ticket
                trade.SuccessfulTrade = true;
                _transactionContext.AddTrade(_mapper.Map<TradeTicket, TransactionResource>(trade));
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