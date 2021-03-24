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

namespace RiskGame.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IPlayerService _playerService;
        private readonly IAssetService _assetService;
        private readonly IShareService _shareService;
        private readonly ITransactionLogic _transactionLogic;
        private readonly IMapper _mapper;
        public TransactionService(IPlayerService playerService, IAssetService assetService, IShareService shareService, IMapper mapper, ITransactionLogic transactionLogic)
        {
            _playerService = playerService;
            _assetService = assetService;
            _shareService = shareService;
            _mapper = mapper;
            _transactionLogic = transactionLogic;
        }

        public async Task<TradeTicket> Transact(TradeTicket trade)
        {
            // get the buyer and seller
            if(trade.Buyer != null && trade.Cash == null)
            {
                trade.Message = "No cash was included in the ticket";
                trade.SuccessfulTrade = false;
                return trade;
            } // OUT buyer has no cash
            if(trade.Seller != null && trade.Shares == null)
            {
                trade.Message = "No shares were included in the ticket";
                trade.SuccessfulTrade = false;
                return trade;
            } // OUT seller has no shares
            // get Haus
            var hausRef = _playerService.GetHAUSRef();
            var haus = _playerService.GetHAUS();
            // get Buyer
            var buyer = new PlayerResource();
            if (trade.Buyer != null) await _playerService.GetAsync(trade.Buyer.Id).Result.ForEachAsync(b => buyer = b);
            else buyer = _mapper.Map<Player,PlayerResource>(haus);
            // get Seller
            var seller = new PlayerResource();
            if (trade.Seller != null) await _playerService.GetAsync(trade.Seller.Id).Result.ForEachAsync(s => seller = s);
            else seller = _mapper.Map<Player, PlayerResource>(haus);
            // get Refs
            var sellerRef = _playerService.ResToRef(seller);
            var buyerRef = _playerService.ResToRef(buyer);
            // get the cash and shares
            var tradeCash = _transactionLogic.GetCash(trade.Cash, trade.CashCount).Result;
            var tradeShares = _transactionLogic.GetShares(trade.Shares, trade.SharesCount).Result;
            try
            {
                // Transfer ownership of cash and assets
                _transactionLogic.TransferShares(buyer, tradeShares, trade.CashCount);
                _transactionLogic.TransferShares(seller, tradeCash, trade.CashCount);
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