using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Services;

namespace RiskGame.API.Logic
{
    public class PlayerLogic : IPlayerLogic
    {
        private readonly IPlayerService _playerService;
        private readonly IAssetService _assetService;
        private readonly IShareService _shareService;
        private readonly IMapper _mapper;
        public PlayerLogic(IMapper mapper, IPlayerService playerService, IAssetService assetService, IShareService shareService)
        {
            _playerService = playerService;
            _assetService = assetService;
            _shareService = shareService;
            _mapper = mapper;
        }
        // REASONS TO TAKE A TURN
        // Actual asset allocation is out of balance
        // Asset appears to be a good buy
        // Asset appears to be a good sell

        public TurnTypes AssetAllocation(Player player)
        {
            var playerRef = _playerService.ToRef(player);
            var playerPortfolio = _shareService.GetPlayerCash(playerRef).Result;
            var playerWallet = _shareService.GetPlayerShares(playerRef, ModelTypes.Share).Result;
            var assets = _assetService.GetAsync().Result;
            var lastBuyPrice = assets[0].LastBuyPrice;
            var lastSellPrice = assets[0].LastSellPrice;
            var portfolioBuy = playerPortfolio.Count * lastBuyPrice;
            var portfolioSell = playerPortfolio.Count * lastSellPrice;
            var currentBuyAllocation = portfolioBuy / (portfolioBuy + playerWallet.Count);
            var currentSellAllocation = portfolioSell / (portfolioSell + playerWallet.Count);
            if (currentBuyAllocation < player.RiskTolerance) return TurnTypes.Buy;
            else if (currentSellAllocation > player.RiskTolerance) return TurnTypes.Sell;
            return TurnTypes.Hold;
        }
        public TurnTypes EvaluateAsset(Player player, Asset asset)
        {
            // look at underlying CompanyAsset
            // look at equity multiplier: A/E
            // if desire and reality differ by enough then make a trade

            return TurnTypes.Hold;
        }
    }
    public interface IPlayerLogic
    {
        TurnTypes AssetAllocation(Player player);
    }
}
