using AutoMapper;
using MongoDB.Driver;
using RiskGame.API.Entities;
using RiskGame.API.Logic;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
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
        private readonly Random randy;
        private readonly IMapper _mapper;
        public EconService(IAssetRepo assetRepo, IEconRepo econRepo, IMarketRepo marketRepo, IEconLogic econLogic, IMapper mapper, IDatabaseSettings dbSettings)
        {
            _mapper = mapper;
            _econRepo = econRepo;
            _marketRepo = marketRepo;
            _econLogic = econLogic;
            _assetRepo = assetRepo;
            randy = new Random();
        }
        public string AssetLoop(Guid econId)
        {
            var marketHistory = new MarketMetricsHistory();
            var econFilter = Builders<EconomyResource>.Filter.Eq("GameId", econId);
            var economy = _econRepo.GetOne(econId);
            var markets = _marketRepo.GetMany().ToList();
            var assets = _assetRepo.GetGameAssets(econId).ToArray();
            var companyAssets = assets.Select(a => a.CompanyAsset).ToArray();
            if (markets.Count == 0) 
                _marketRepo.CreateOne(_mapper.Map<Market, MarketResource>(new Market(econId, assets.Select(a => a.CompanyAsset).ToArray(), randy, null)));
            do
            {
                var loop = new MarketLoopData
                {
                    EconId = econId,
                    Economy = economy,
                    Assets = assets,
                    LastMarket = markets.LastOrDefault()
                };
                var next = _econLogic.LoopRound(loop);
                _econRepo.ReplaceOne(Builders<EconomyResource>.Filter.Eq("GameId", econId), next.Economy);
                _assetRepo.ReplaceOne(Guid.Parse(assets[0].AssetId), next.Assets[0]);
                _marketRepo.CreateOne(next.LastMarket);
                economy = next.Economy;
                marketHistory.Red.Add(next.Market.Red * (int)next.Market.RedDirection);
                marketHistory.Orange.Add(next.Market.Orange * (int)next.Market.OrangeDirection);
                marketHistory.Yellow.Add(next.Market.Yellow * (int)next.Market.YellowDirection);
                marketHistory.Green.Add(next.Market.Green * (int)next.Market.GreenDirection);
                marketHistory.Blue.Add(next.Market.Blue * (int)next.Market.BlueDirection);
                marketHistory.Violet.Add(next.Market.Violet * (int)next.Market.VioletDirection);
                //Thread.Sleep(1);
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
