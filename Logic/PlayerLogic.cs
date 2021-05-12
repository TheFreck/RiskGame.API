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
            var portQty = portfolio.Where(s => s._assetId == asset.AssetId).Where(s => s.CurrentOwner.Id == player.PlayerId).Count();
            var portfolioValue = portQty * lastPrice;
            double portfolioAllocation = (double)(portfolioValue/(portfolioValue + player.Cash));

            var allocationQty = (int)Math.Ceiling((decimal)player.RiskTolerance * (player.Cash + portfolioValue)/lastPrice - portQty);
            var evaluationQty = (int)Math.Ceiling(portQty * (decision.Price - lastPrice) / lastPrice);
            decision.Qty = allocationQty > 0 && evaluationQty > 0 ? Math.Max(allocationQty, evaluationQty) : allocationQty < 0 && evaluationQty < 0 ? Math.Min(allocationQty, evaluationQty) : allocationQty + evaluationQty;
            
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
            var successRatio = (.7 * news.PrimarySuccessRatio + .3 * news.SecondarySuccessRatio) + 1;
            var weightedGrowth = (.7 * news.PrimaryGrowth + .3 * news.MarketGrowth);
            var currentValueEstimate = (decimal)Math.Pow(news.LastDividendValue * (1 + successRatio * weightedGrowth),asset.PeriodsSinceDividend);
            
            decision.Price = currentValueEstimate;
            return decision;
        }
    }
    public interface IPlayerLogic
    {
        PlayerDecision PlayerTurn(PlayerResource player, Share[] portfolio, AssetResource[] assets, MarketMetricsHistory history);
    }
}
