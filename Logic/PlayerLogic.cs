using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.MarketFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Services;

namespace RiskGame.API.Logic
{
    public class PlayerLogic : IPlayerLogic
    {
        private readonly IMapper _mapper;
        public PlayerLogic(IMapper mapper)
        {
            _mapper = mapper;
        }
        public PlayerDecision PlayerTurn(PlayerResource player, Share[] portfolio, AssetResource[] assets, MarketMetricsHistory history)
        {
            var decision = new PlayerDecision();
            return EvaluateAsset(player, assets, history, Grapevine(AssetAllocation(player, portfolio, assets, decision)));
        }
        public PlayerDecision AssetAllocation(PlayerResource player, Share[] portfolio, AssetResource[] assets, PlayerDecision decision)
        {
            var playerRef = _mapper.Map<PlayerResource,ModelReference >(player);
            var cash = assets.Where(a => a.ModelType == ModelTypes.Cash).FirstOrDefault();
            var playerWallet = player.Cash;
            var playerPortfolio = portfolio;
            var asset = assets.Where(a => a.ModelType == ModelTypes.Asset).FirstOrDefault();
            var lastBuyPrice = asset.LastBuyPrice;
            var lastSellPrice = asset.LastSellPrice;
            var lastPrice = (lastBuyPrice + lastSellPrice) / 2;
            var portfolioValue = playerPortfolio.Where(s => s._assetId == asset.AssetId).Count() * lastPrice;
            double portfolioAllocation = portfolioValue/(portfolioValue + playerWallet);
            int turnType = (int)Math.Floor(
                (portfolioAllocation - player.RiskTolerance) / (.1 * player.RiskTolerance)) > 2 ?
                2 : 
                (int)Math.Floor((portfolioAllocation - player.RiskTolerance) / (.1 * player.RiskTolerance)) < -2 ? 
                -2 : 
                (int)Math.Floor((portfolioAllocation - player.RiskTolerance) / (.1 * player.RiskTolerance));
            decision.Allocation = (TurnTypes)turnType;
            decision.Qty = (int)Math.Ceiling((playerWallet * player.RiskTolerance) / (1 - player.RiskTolerance)) - playerPortfolio.Count();
            return decision;
        }
        public PlayerDecision Grapevine(PlayerDecision decision)
        {
            // check on other traders
            decision.Grapevine = TurnTypes.Hold;
            return decision;
        }
        public PlayerDecision EvaluateAsset(PlayerResource player, AssetResource[] assets, MarketMetricsHistory history, PlayerDecision decision)
        {
            var recommendation = 0;
            var news = new Newspaper(history).ReadNewspaper(player.Experience, assets[0]);
            var successRatio = (.7 * news.PrimarySuccessRatio + .3 * news.SecondarySuccessRatio);
            var weightedGrowth = (.7 * news.PrimaryGrowth + .3 * news.MarketGrowth);
            var currentValueEstimate = news.LastDividendValue * (1 + weightedGrowth * news.CyclesSinceLastDividend);
            if(currentValueEstimate > assets[0].LastBuyPrice)
            {
                recommendation++;
            }
            if(currentValueEstimate < assets[0].LastSellPrice)
            {
                recommendation--;
            }
            if (successRatio > .5) recommendation++;
            else recommendation--;
            decision.Value = currentValueEstimate;

            return decision;
        }
    }
    public interface IPlayerLogic
    {
        PlayerDecision PlayerTurn(PlayerResource player, Share[] portfolio, AssetResource[] assets, MarketMetricsHistory history);
        PlayerDecision AssetAllocation(PlayerResource player, Share[] portfolio, AssetResource[] assets, PlayerDecision decision);
        PlayerDecision Grapevine(PlayerDecision decision);
        PlayerDecision EvaluateAsset(PlayerResource player, AssetResource[] assets, MarketMetricsHistory history, PlayerDecision decision);
    }
}
