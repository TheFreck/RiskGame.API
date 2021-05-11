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
                    LastMarket = market
                };
                var next = _econLogic.LoopRound(loop);
                _econRepo.ReplaceOne(econFilter, next.Economy);
                _assetRepo.ReplaceOne(assets[0].AssetId, next.Assets[0]);
                _marketRepo.CreateOne(next.LastMarket);
                economy = next.Economy;
                timer.Stop();
                Console.WriteLine("econ: " + timer.ElapsedMilliseconds);
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
