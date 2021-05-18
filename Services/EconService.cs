using AutoMapper;
using MongoDB.Driver;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Logic;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Persistence;
using RiskGame.API.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RiskGame.API.Services
{
    public class EconService : IEconService
    {
        private readonly IEconRepo _econRepo;
        private readonly IMarketRepo _marketRepo;
        private readonly IAssetRepo _assetRepo;
        private readonly IEconLogic _econLogic;
        private readonly IAssetLogic _assetLogic;
        private readonly IShareRepo _shareRepo;
        private readonly IPlayerRepo _playerRepo;
        private readonly Random randy;
        private readonly IMapper _mapper;
        public EconService(IAssetRepo assetRepo, IEconRepo econRepo, IMarketRepo marketRepo, IEconLogic econLogic, IAssetLogic assetLogic, IShareRepo shareRepo, IPlayerRepo playerRepo, IMapper mapper, IDatabaseSettings dbSettings)
        {
            _mapper = mapper;
            _econRepo = econRepo;
            _marketRepo = marketRepo;
            _econLogic = econLogic;
            _assetRepo = assetRepo;
            _assetLogic = assetLogic;
            _shareRepo = shareRepo;
            _playerRepo = playerRepo;
            randy = new Random();
        }
        public string AssetLoop(Guid econId)
        {
            var econFilter = Builders<EconomyResource>.Filter.Eq("GameId", econId);
            var economy = _econRepo.GetOne(econId);
            var assets = _assetRepo.GetGameAssets(econId).ToArray();
            var companyAssets = assets.Select(a => a.CompanyAsset).ToArray();
            var market = _mapper.Map<Market, MarketResource>(new Market(econId, companyAssets, randy, null));
            do
            {
                var timer = new Stopwatch();
                timer.Start();
                var loop = new MarketLoopData
                {
                    EconId = econId,
                    Economy = economy,
                    Assets = assets,
                    Market = market
                };
                var next = _econLogic.LoopRound(loop);
                var asset = next.Assets[0];
                if(asset.PeriodsSinceDividend == 20)
                {
                    Console.WriteLine("dividend day!!!");
                    var shares = _shareRepo.GetMany().Where(s => s._assetId == next.Assets[0].AssetId).ToArray();
                    // calculate dividend
                    asset.PeriodsSinceDividend = 0;
                    var incomeSheet = _assetLogic.CalculateDividend(asset);
                    var dividendsPerShare = incomeSheet.Dividends / shares.Length;
                    asset.TradeHistory.Add(Tuple.Create(TradeType.Dividend, asset.TradeHistory.OrderByDescending(t => t.Item1).FirstOrDefault().Item2 - dividendsPerShare));
                    asset.LastDividendPayout = incomeSheet.Dividends;
                    asset.MostRecentValue = asset.CompanyAsset.Value * asset.Debt;
                    // pay dividends on each share
                    var dividendPaymentSchedule = new Dictionary<Guid, decimal>();
                    foreach (var share in shares)
                    {
                        if (!dividendPaymentSchedule.ContainsKey(share.CurrentOwner.Id)) dividendPaymentSchedule.Add(share.CurrentOwner.Id, 0);
                        dividendPaymentSchedule[share.CurrentOwner.Id] += dividendsPerShare;
                    }
                    var players = new List<PlayerResource>();
                    foreach(var key in dividendPaymentSchedule)
                    {
                        var player = _playerRepo.GetOne(key.Key);
                        player.Cash += key.Value;
                        players.Add(player);
                        _playerRepo.UpdateOne(player.PlayerId, Builders<PlayerResource>.Update.Set("Cash", player.Cash));
                    }
                }

                asset.CompanyAsset.Value = asset.MostRecentValue / asset.Debt;

                asset.CompanyAssetValuePerShare = asset.MostRecentValue / asset.SharesOutstanding; ;
                _econRepo.ReplaceOne(econFilter, next.Economy); // update instead
                var assetUpdateBase = Builders<AssetResource>.Update;
                var assetUpdate = assetUpdateBase
                    .Set("CompanyAsset", asset.CompanyAsset)
                    .Set("PeriodsSinceDividend", asset.PeriodsSinceDividend)
                    .Set("TradeHistory", asset.TradeHistory)
                    .Set("LastDividendPayout", asset.LastDividendPayout)
                    .Set("CompanyAssetValuePerShare", asset.CompanyAssetValuePerShare);
                var updated = _assetRepo.UpdateOne(asset.AssetId, assetUpdate).Result;
                _marketRepo.CreateOne(next.Market);
                economy = next.Economy;
                timer.Stop();
                //Console.WriteLine("econ: " + timer.ElapsedMilliseconds);
                Thread.Sleep(1000);
            } while (IsRunning(econId));
            Console.WriteLine("Finito");
            return "that was fun wasn't it?";
        }
        public bool IsRunning(Guid gameId) => _econRepo.GetOne(gameId).isRunning;
        public EconomyResource GetGame(Guid gameId) => _econRepo.GetOne(gameId);
        public List<EconomyOut> GetGames() => _mapper.Map<List<EconomyResource>,List<EconomyOut>>(_econRepo.GetAll().ToList());
    }
    public interface IEconService
    {
        string AssetLoop(Guid econId);
        bool IsRunning(Guid gameId);
        List<EconomyOut> GetGames();
        EconomyResource GetGame(Guid gameId);
    }
}
