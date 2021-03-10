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

        public TradeTicket Transact(TradeTicket trade)
        {
            var buyer = _playerService.Get(trade.Buyer.Id);
            var buyerCash = buyer.Cash;
            if (buyerCash < trade.Price * trade.Shares.Length)
            {
                trade.SuccessfulTrade = false;
                trade.Message = "buyer does not have enough cash for this puchase";
                return trade;
            }
            var seller = _playerService.Get(trade.Seller.Id);
            var shares = new List<Share>();

            // Transfer ownership of the assets
            foreach (var shareRef in trade.Shares)
            {
                var share = _shareService.Get(shareRef.Id);
                if (share.CurrentOwner.Id == seller.Id)
                {
                    share.CurrentOwner = new ModelReference {Id = buyer.Id,Name = buyer.Name,ModelType = ModelTypes.Player };
                    seller.Portfolio.Remove(shareRef);
                    buyer.Portfolio.Add(shareRef);
                    share.CurrentOwner = _mapper.Map<Player, ModelReference>(buyer);
                    shares.Add(share);
                }
                else
                {
                    trade.SuccessfulTrade = false;
                    trade.Message = "The seller does not have the assets to sell";
                    return trade;
                }
            }
            trade.Message = _shareService.UpdateShares(shares).Message;
            trade.TradeTime = DateTime.Now;
            trade.SuccessfulTrade = true;
            return trade;
            //foreach(var item in seller.Portfolio.Where(a => assets.Contains(a)))
            //{
            //    seller.Portfolio.Remove(item);

            //}

        }
    }
}