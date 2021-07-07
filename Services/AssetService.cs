using MongoDB.Driver;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Persistence.Repositories;

namespace RiskGame.API.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepo _assetRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IPlayerRepo _playerRepo;
        private readonly IMarketRepo _marketRepo;
        private readonly IEconRepo _econRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IMapper _mapper;
        public AssetService(IAssetRepo assetRepo, IPlayerRepo playerRepo, IShareRepo shareRepo, IMarketRepo marketRepo, IEconRepo econRepo, ITransactionRepo transactionRepo, IMapper mapper)
        {
            _assetRepo = assetRepo;
            _playerRepo = playerRepo;
            _shareRepo = shareRepo;
            _marketRepo = marketRepo;
            _econRepo = econRepo;
            _transactionRepo = transactionRepo;
            _mapper = mapper;

        }
        public List<AssetResource> GetAssets() => _assetRepo.GetMany().ToList();
        public CompanyAsset[] GetCompanyAssets(Guid gameId) => _assetRepo.GetMany().Where(a => a.GameId == gameId).Select(a => a.CompanyAsset).ToArray();
        public IQueryable<ShareResource> GetShares(Guid assetId)
        {
            var many = _shareRepo.GetMany().Where(s => s._assetId == assetId).AsQueryable();
            return many;
        }
        public AssetResource GetAsset(Guid id, ModelTypes type) => _assetRepo.GetMany()
            .Where(a => a.AssetId == id)
            .Where(a => a.ModelType == type)
            .FirstOrDefault();
        public AssetResource[] GetGameAssets(Guid id) => _assetRepo.GetMany().Where(a => a.GameId == id).Where(a => a.CompanyAsset != null).ToArray();
        public ChartPixel GetAssetPrices(Guid gameId, Guid assetId, int frame)
        {
            var trades = _assetRepo.GetMany().Where(a => a.GameId == gameId).Where(a => a.AssetId == assetId).Select(a => a.TradeHistory).FirstOrDefault();
            var pixelTrades = trades.OrderBy(d => d.Item1).TakeLast(trades.Count - frame);
            return new ChartPixel
            {
                Volume = pixelTrades.Count(),
                Open = pixelTrades.OrderBy(a => a.Item1).FirstOrDefault().Item3,
                Close = pixelTrades.OrderBy(a => a.Item1).LastOrDefault().Item3,
                High = pixelTrades.OrderByDescending(t => t.Item3).FirstOrDefault().Item3,
                Low = pixelTrades.OrderByDescending(t => t.Item3).LastOrDefault().Item3,
                LastFrame = trades.Count()
            };
        }
        public List<ChartPixel> GetTrades(Guid gameId, Guid assetId, DateTime since)
        {
            var tradesOut = new List<ChartPixel>();
            var trades = _transactionRepo.GetTradesSince(gameId, assetId, since);
            var allTrades = trades.GroupBy(t => t.TradeTime.Second);
            foreach(var tradeGroup in allTrades)
            {
                tradesOut.Add(new ChartPixel
                {
                    TimeOpen = tradeGroup.OrderByDescending(t => t.Sequence).FirstOrDefault().TradeTime,
                    Open = tradeGroup.OrderByDescending(t => t.Sequence).FirstOrDefault().Price,
                    Close = tradeGroup.OrderBy(t => t.Sequence).FirstOrDefault().Price,
                    High = tradeGroup.OrderByDescending(t => t.Price).FirstOrDefault().Price,
                    Low = tradeGroup.OrderBy(t => t.Price).FirstOrDefault().Price,
                    Volume = tradeGroup.Count(),
                    TimeClose = tradeGroup.OrderBy(t => t.Sequence).FirstOrDefault().TradeTime
                });
            }
            return tradesOut;
        }
        public async Task<string> Create(AssetResource asset) => await _assetRepo.CreateOne(asset);
        public void Remove(AssetResource assetIn) => _assetRepo.DeleteOne(assetIn.AssetId);
        public void RemoveFromGame(Guid assetId) => _assetRepo.DeleteOne(assetId);
        public void RemoveAssetsFromGame(List<Guid> assetIds) => _assetRepo.DeleteMany(assetIds);
        public ModelReference ToRef(Asset asset) => _mapper.Map<Asset, ModelReference>(asset);
        public ModelReference ResToRef(AssetResource asset) => _mapper.Map<AssetResource, ModelReference>(asset);

    }
    public interface IAssetService
    {
        //string Initialize();
        List<AssetResource> GetAssets();
        CompanyAsset[] GetCompanyAssets(Guid gameId);
        IQueryable<ShareResource> GetShares(Guid id);
        AssetResource GetAsset(Guid id, ModelTypes type);
        AssetResource[] GetGameAssets(Guid id);
        ChartPixel GetAssetPrices(Guid gameId, Guid assetId, int frame);
        List<ChartPixel> GetTrades(Guid gameId, Guid assetId, DateTime since);
        Task<string> Create(AssetResource asset);
        void Remove(AssetResource assetIn);
        void RemoveFromGame(Guid assetId);
        void RemoveAssetsFromGame(List<Guid> assetIds);
        ModelReference ToRef(Asset asset);
        ModelReference ResToRef(AssetResource asset);
    }
}