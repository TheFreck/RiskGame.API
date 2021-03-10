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
            var buyer = _playerService.Get(trade.Buyer.Id);
            var seller = _playerService.Get(trade.Seller.Id);
            var cashIds = _mapper.Map<List<ModelReference>, List<SimpleId>>(trade.Cash);
            var cashGuids = new List<Guid>();
            foreach(var id in cashIds)
            {
                cashGuids.Add(id.Id);
            }
            var buyerCash = await _shareService.GetAsync(cashGuids);
            var shareIds = _mapper.Map<List<ModelReference>, List<SimpleId>>(trade.Shares);
            var shareGuids = new List<Guid>();
            foreach(var id in shareIds)
            {
                shareGuids.Add(id.Id);
            }

            var tradeShares = await _shareService.GetAsync(shareGuids);
            
            // Transfer ownership of cash
            foreach(var cash in buyerCash)
            {
                var cashRef = _mapper.Map<Share, ModelReference>(cash);
                cash.CurrentOwner = _mapper.Map<Player,ModelReference>(seller);
                seller.Cash.Add(cashRef);
                buyer.Cash.Remove(cashRef);
            }

            // Transfer ownership of assets
            foreach (var tradeShare in tradeShares)
            {
                var shareRef = _mapper.Map<Share, ModelReference>(tradeShare);
                tradeShare.CurrentOwner = _mapper.Map<Player, ModelReference>(buyer);
                buyer.Portfolio.Add(shareRef);
                buyer.Portfolio.Remove(shareRef);
            }
            try
            {
                trade.Message = $"Shares: {_shareService.UpdateShares(tradeShares).Message}; Cash: {_shareService.UpdateShares(buyerCash)}";
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
            //foreach(var item in seller.Portfolio.Where(a => assets.Contains(a)))
            //{
            //    seller.Portfolio.Remove(item);

            //}

        }
    }
}