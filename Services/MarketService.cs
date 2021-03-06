using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Entities;
using RiskGame.API.Models.PlayerFolder;
using MongoDB.Driver;
using RiskGame.API.Persistence;
using RiskGame.API.Models.EconomyFolder;
using AutoMapper;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Mappings;
using RiskGame.API.Models.MarketFolder;
using MongoDB.Bson;
using System.Threading;
using RiskGame.API.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Persistence.Repositories;

namespace RiskGame.API.Services
{
    public class MarketService : IMarketService
    {
        private readonly IAssetRepo _assetRepo;
        private readonly IPlayerRepo _playerRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IMarketRepo _marketRepo;
        private readonly IEconRepo _econRepo;
        private readonly IEconService _econService;
        private readonly TransactionContext _transactionContext;
        private readonly Random randy;
        private readonly IMapper _mapper;

        public MarketService(IMapper mapper, IEconService econService, IAssetRepo assetRepo, IPlayerRepo playerRepo, IShareRepo shareRepo, IMarketRepo marketRepo, IEconRepo econRepo, TransactionContext transactionContext)
        {
            _mapper = mapper;
            randy = new Random();
            _assetRepo = assetRepo;
            _playerRepo = playerRepo;
            _shareRepo = shareRepo;
            _marketRepo = marketRepo;
            _econRepo = econRepo;
            _econService = econService;
            _transactionContext = transactionContext;
        }
        public ChartPixel GetRecords(Guid gameId, int lastSequence)
        {
            var records = _marketRepo.GetMany().Where(m => (m.SequenceNumber > lastSequence) && (m.GameId == gameId)).OrderByDescending(m => m.SequenceNumber).ToList();
            var pixel = new ChartPixel();
            pixel.LastFrame = lastSequence;
            if(records.Count > 4)
            {
                pixel.Open = records.LastOrDefault().Assets[0] != null ? records.LastOrDefault().Assets[0].Value : records.LastOrDefault().Assets[1].Value;
                pixel.Close = records.FirstOrDefault().Assets[0] != null ? records.FirstOrDefault().Assets[0].Value : records.FirstOrDefault().Assets[1].Value;
                pixel.LastFrame = records.FirstOrDefault().SequenceNumber;
                var ascendingValue = records.OrderBy(m => m.Assets[0] != null ? m.Assets[0].Value : m.Assets[1].Value);
                pixel.High = ascendingValue.LastOrDefault().Assets[0] != null ? ascendingValue.LastOrDefault().Assets[0].Value: ascendingValue.LastOrDefault().Assets[1].Value;
                pixel.Low = ascendingValue.FirstOrDefault().Assets[0] != null ? ascendingValue.FirstOrDefault().Assets[0].Value : ascendingValue.FirstOrDefault().Assets[1].Value;
                pixel.Volume = records.Count;
            }
            return pixel;
        }
        //public UpdateResult SetPixelCount(Guid gameId, int count) {
        //    var update = Builders<EconomyResource>.Update.Set("PixelCount", count);
        //    return _econRepo.UpdateOne(gameId,update).Result;
        //}
        //public UpdateResult SetTrendiness(Guid gameId, int trend)
        //{
        //    var update = Builders<EconomyResource>.Update.Set("Trendiness", trend);
        //    return _econRepo.UpdateOne(gameId, update).Result;
        //}
        public void AssetsStartStop(Guid gameId, bool running)
        {
            var update = Builders<EconomyResource>.Update.Set("isRunning", running);
            var game = _econRepo.UpdateOne(gameId,update);
            _econService.AssetLoop(gameId);
        }
        public string BigBang(string secretCode) => _econRepo.DeleteAll(secretCode) ? "the screams of the deleted data are beginning to fade... \n ...somewhere in the distance can be heard the sound of memory being reallocated... \n ...and that's the fan turning on" : "Not gonna do it";
        public async Task<string> EndGame(Guid gameId)
        {
            // Stop game
            var econFilter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var econUpdate  = Builders<EconomyResource>.Update.Set("isRunning", false);
            await _econRepo.UpdateOne(gameId, econUpdate);
            // destroy assets and their shares
            foreach(var asset in _assetRepo.GetGameAssets(gameId))
            {
                await _shareRepo.DeleteAssetShares(asset.AssetId);
            }
            await _assetRepo.DeleteGameAssets(gameId);
            // delete the markets
            var markets = _marketRepo.DeleteGameMarkets(gameId);
            // players' turns
            await _playerRepo.DeleteGamePlayers(gameId);
            await _econRepo.DeleteOne(gameId);
            return "there is nothing left; just the rubble of bits and bytes to be reallocated";
        }
        public Guid NewGame(Guid gameId, AssetResource[] assets)
        {
            _transactionContext.CleanSlate();
            var newGame = new Economy(gameId);
            newGame.Markets = new List<Tuple<DateTime, MarketMetrics>>();
            newGame.Assets = assets.Select(a => a.CompanyAsset).ToArray();
            var newMarket = new Market();
            newGame.Markets.Add(new Tuple<DateTime, MarketMetrics>(DateTime.Now, newMarket.GetMetrics(assets.Select(a => a.CompanyAsset).ToArray())));
            _econRepo.CreateOne(_mapper.Map<Economy,EconomyResource>(newGame));
            return newGame.GameId;
        }
        public CompanyAsset[] GetCompanyAssets(Guid gameId) => _assetRepo.GetGameAssets(gameId).Select(a => a.CompanyAsset).ToArray();
        public Economy GetGame(Guid gameId) => _mapper.Map<EconomyResource,Economy>(_econRepo.GetOne(gameId));
        public string UpdateGame(Guid econId, UpdateDefinition<EconomyResource> update) => _econRepo.UpdateOne(econId, update).Result.ToString();
        public List<MarketMetrics> GetMarkets(Guid gameId) => _mapper.Map<List<MarketResource>,List<MarketMetrics>>(_marketRepo.GetMany().Where(m => m.GameId == gameId).ToList());
    }
    public interface IMarketService
    {
        string BigBang(string secretCode);
        Task<string> EndGame(Guid gameId);
        ChartPixel GetRecords(Guid gameId, int lastSequence);
        //UpdateResult SetPixelCount(Guid gameId, int count);
        //UpdateResult SetTrendiness(Guid gameId, int trend);
        void AssetsStartStop(Guid gameId, bool running);
        Guid NewGame(Guid gameId, AssetResource[] assets);
        CompanyAsset[] GetCompanyAssets(Guid gameId);
        Economy GetGame(Guid gameId);
        string UpdateGame(Guid econId, UpdateDefinition<EconomyResource> update);
        List<MarketMetrics> GetMarkets(Guid gameId);
    }
}
