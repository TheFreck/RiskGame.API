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

namespace RiskGame.API.Services
{
    public class TransactionService
    {
        private readonly PlayerService _playerService;
        private readonly AssetService _assetService;
        private readonly ShareService _shareService;
        private readonly IMapper _mapper;
        public TransactionService(PlayerService playerService, AssetService assetService, ShareService shareService, IMapper mapper)
        {
            _playerService = playerService;
            _assetService = assetService;
            _shareService = shareService;
            _mapper = mapper;
        }

        public async Task<TradeTicket> Transact(TradeTicket trade)
        {
            var incomingBuyer = _playerService.GetAsync(trade.Buyer.Id);
            var buyer = new Player();
            await incomingBuyer.Result.ForEachAsync(b => buyer = b);
            var incomingSeller = _playerService.GetAsync(trade.Seller.Id);
            var seller = new Player();
            await incomingSeller.Result.ForEachAsync(s => seller = s);

            var cashGuids = new List<Guid>();
            foreach(var id in buyer.Wallet)
            {
                cashGuids.Add(id.Id);
            }
            var incomingCash = await _shareService.GetAsync(cashGuids);
            var tradeCash = new List<Share>();
            await incomingCash.ForEachAsync(c => tradeCash.Add(c));

            var shareGuids = new List<Guid>();
            foreach(var id in trade.Shares)
            {
                shareGuids.Add(id.Id);
            }

            var incomingShares = await _shareService.GetAsync(shareGuids);
            var tradeShares = new List<Share>();
            await incomingShares.ForEachAsync(s => tradeShares.Add(s));
            
            // Transfer ownership of cash
            foreach(var cash in tradeCash)
            {
                var cashRef = _mapper.Map<Share, ModelReference>(cash);
                cash.CurrentOwner = _mapper.Map<Player,ModelReference>(seller);
                foreach(var bill in buyer.Wallet)
                {
                    if(cash.Id == bill.Id)
                    {
                        buyer.Wallet.Remove(bill);
                        break;
                    }
                }
                seller.Wallet.Add(cashRef);
            }

            // Transfer ownership of assets
            foreach (var tradeShare in tradeShares)
            {
                var shareRef = _mapper.Map<Share, ModelReference>(tradeShare);
                tradeShare.CurrentOwner = _mapper.Map<Player, ModelReference>(buyer);
                buyer.Portfolio.Add(shareRef);
                seller.Portfolio.Remove(shareRef);
            }
            try
            {
                // update buyer and seller
                _playerService.Update(buyer.Id, buyer);
                _playerService.Update(seller.Id, seller);
                trade.Message = $"Shares: {_shareService.UpdateShares(tradeShares).Message}; Cash: {_shareService.UpdateShares(tradeCash)}";
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
}