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
        public PlayerDecision PlayerTurn(PlayerResource player, Share[] portfolio, AssetResource[] assets, MarketMetricsHistory history) => AssetAllocation(player, portfolio, assets, Grapevine(EvaluateAsset(player, assets, history, new PlayerDecision())));
        private PlayerDecision AssetAllocation(PlayerResource player, Share[] portfolio, AssetResource[] assets, PlayerDecision decision)
        {
            var asset = assets.Where(a => a.ModelType == ModelTypes.Asset).FirstOrDefault();
            var price = decision.Price;
            var lastPrice = asset.TradeHistory.OrderByDescending(t => t.Item1).LastOrDefault().Item2;
            decimal playerWallet = player.Cash;

            var portfolioValue = portfolio.Where(s => s._assetId == asset.AssetId).Where(s => s.CurrentOwner.Id == player.PlayerId).Count() * lastPrice;
            double portfolioAllocation = (double)(portfolioValue/(portfolioValue + playerWallet));

            if(Math.Abs(portfolioAllocation - player.RiskTolerance) > .2)
            {
                decision.Qty = (int)Math.Ceiling(((double)playerWallet * player.RiskTolerance) / (1 - player.RiskTolerance)) - portfolio.Count();
            }
            var turnType = portfolioAllocation - player.RiskTolerance > .2 ? 1 : portfolioAllocation - player.RiskTolerance < -.2 ? -1 : 0;
            decision.Asset = _mapper.Map<AssetResource, ModelReference>(asset);
            return decision;
        }
        private PlayerDecision Grapevine(PlayerDecision decision)
        {
            // check on other traders
            // if there's excitement among other traders then change a Buy into a QuickBuy and a Sell into a QuickSell
            return decision;
        }
        private PlayerDecision EvaluateAsset(PlayerResource player, AssetResource[] assets, MarketMetricsHistory history, PlayerDecision decision)
        {
            var asset = assets.Where(a => a.ModelType == ModelTypes.Asset).FirstOrDefault();
            decision.Asset = _mapper.Map<AssetResource,ModelReference>(asset);
            var news = new Newspaper(history).ReadNewspaper(player.Experience, asset);
            var successRatio = (.7 * news.PrimarySuccessRatio + .3 * news.SecondarySuccessRatio);
            var weightedGrowth = (.7 * news.PrimaryGrowth + .3 * news.MarketGrowth);
            decimal currentValueEstimate = (decimal)(news.LastDividendValue * (1 + successRatio * weightedGrowth * news.CyclesSinceLastDividend)) / assets[0].SharesOutstanding;
            
            decision.Price = (double)currentValueEstimate;
            if (currentValueEstimate < assets[0].TradeHistory.OrderByDescending(a => a.Item1).FirstOrDefault().Item2) decision.Action = TurnTypes.Sell;
            else decision.Action = TurnTypes.Buy;
            return decision;
        }
    }
    public interface IPlayerLogic
    {
        PlayerDecision PlayerTurn(PlayerResource player, Share[] portfolio, AssetResource[] assets, MarketMetricsHistory history);
    }
}
