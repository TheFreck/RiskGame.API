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

namespace RiskGame.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IPlayerService _playerService;
        private readonly IAssetService _assetService;
        private readonly IShareService _shareService;
        private readonly ITransactionLogic _transactionLogic;
        private readonly IMarketService _marketService;
        private readonly IMapper _mapper;
        public TransactionService(IPlayerService playerService, IAssetService assetService, IShareService shareService, IMarketService marketService, IMapper mapper, ITransactionLogic transactionLogic)
        {
            _playerService = playerService;
            _assetService = assetService;
            _shareService = shareService;
            _mapper = mapper;
            _transactionLogic = transactionLogic;
            _marketService = marketService;
        }

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
            } // OUT buyer has insufficient cash
            if(trade.Seller != null && trade.Shares == 0)
            {
                trade.Message = "No shares were included in the ticket";
                trade.SuccessfulTrade = false;
                return trade;
            } // OUT seller has insufficient shares
            // get Haus
            var haus = _playerService.GetHAUS(trade.GameId).Result;
            // get Buyer
            var buyer = new PlayerResource();
            if (trade.Buyer != null) await _playerService.GetPlayerAsync(trade.Buyer.Id).Result.ForEachAsync(b => buyer = b);
            else buyer = _mapper.Map<Player,PlayerResource>(haus);
            // get Seller
            var seller = new PlayerResource();
            if (trade.Seller != null) await _playerService.GetPlayerAsync(trade.Seller.Id).Result.ForEachAsync(s => seller = s);
            else seller = _mapper.Map<Player,PlayerResource>(haus);
            // get References
            var sellerRef = _playerService.ResToRef(seller);
            var buyerRef = _playerService.ResToRef(buyer);
            // get cash and shares
            var tradeAsset = _assetService.GetAsset(trade.Asset.Id, ModelTypes.Asset);
            var cash = _assetService.GetAsset(trade.GameId, ModelTypes.Cash);
            var tradeCash = _shareService.GetPlayerShares(buyerRef, tradeAsset, trade.Cash);
            var tradeShares = _shareService.GetPlayerShares(sellerRef, cash, trade.Shares);
            try
            {
                // Transfer ownership of cash and shares
                var transferredShares = _transactionLogic.TransferShares(buyer, tradeShares, trade.Cash);
                var transferredCash = _transactionLogic.TransferShares(seller, tradeCash, trade.Cash);
                await _shareService.UpdateShares(transferredShares);
                await _shareService.UpdateShares(transferredCash);

                // complete the trade ticket
                trade.Message = $"Shares: {_shareService.UpdateShares(tradeShares).Result.Message}; Cash: {_shareService.UpdateShares(tradeCash)}";
                trade.TradeTime = DateTime.Now;
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
    }
    public interface ITransactionService
    {
        Task<TradeTicket> Transact(TradeTicket trade);
    }
}