using AutoMapper;
using MongoDB.Driver;
using RiskGame.API.Entities;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using RiskGame.API.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RiskGame.API.Services
{
    public class EconService : IEconService
    {
        private readonly IAssetService _assetService;
        private readonly IPlayerService _playerService;
        private readonly IShareService _shareService;
        private readonly Random randy;
        private readonly IMapper _mapper;
        private readonly IDatabaseSettings _dbSettings;
        private readonly IMongoCollection<EconomyResource> _economy;
        private readonly IMongoCollection<MarketResource> _market;
        public EconService(IDatabaseSettings settings, IAssetService assetService, IPlayerService playerService, IShareService shareService, IMapper mapper, IDatabaseSettings dbSettings)
        {
            _dbSettings = dbSettings;
            _mapper = mapper;
            randy = new Random();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            database.DropCollection(settings.EconomyCollectionName);
            _market = database.GetCollection<MarketResource>(settings.MarketCollectionName);
            _economy = database.GetCollection<EconomyResource>(settings.EconomyCollectionName);
            _assetService = assetService;
            _playerService = playerService;
            _shareService = shareService;
        }
        public async Task<string> Motion(Guid econId)
        {
            var econFilter = Builders<EconomyResource>.Filter.Eq("GameId", econId);
            var economy = new EconomyResource();
            var keepGoing = false;
            do
            {
                var companyAssetList = new List<CompanyAsset>();
                var assetsList = new List<AssetResource>();
                var assets = _assetService.GetGameAssetsAsync(econId);
                await assets.ForEachAsync(a => { assetsList.Add(a); companyAssetList.Add(a.CompanyAsset); });
                keepGoing = false;
                var markets = _market.AsQueryable().Where(m => m.GameId == econId).ToList();
                var incomingEconomy = _economy.FindAsync(econFilter).Result;
                await incomingEconomy.ForEachAsync(e => economy = e);
                if (!economy.isRunning) goto LoopEnd;
                var lastMarket = new Market(econId, companyAssetList.ToArray(), _mapper.Map<MarketResource, MarketMetrics>(markets.LastOrDefault()), randy);
                var grownAssets = GrowAssets(assetsList.ToArray(), lastMarket);
                var lastMarketMetrics = lastMarket.GetMetrics(_mapper.Map<AssetResource[], CompanyAsset[]>(grownAssets));
                var nextMarket = new Market(econId, lastMarket.Assets, lastMarketMetrics, randy);
                nextMarket.SequenceNumber = lastMarket.SequenceNumber + 1;
                _market.InsertOne(_mapper.Map<Market, MarketResource>(nextMarket));
                // process players' turns
                Console.WriteLine("tick: " + DateTime.Now.Second + ":" + DateTime.Now.Millisecond);
                // finalizing
                keepGoing = await IsRunning(economy.GameId);
                Thread.Sleep(1);
            } while (keepGoing);
        LoopEnd:
            Console.WriteLine("Finito");
            return "that was fun wasn't it?";
        }

        private AssetResource[] GrowAssets(AssetResource[] assets, Market market)
        {
            foreach (var asset in assets)
            {
                if (asset.CompanyAsset == null) continue;
                var value = asset.CompanyAsset.Value * GrowthRate(asset.CompanyAsset.Value, market.GetMetric(asset.CompanyAsset.PrimaryIndustry), market.GetMetric(asset.CompanyAsset.SecondaryIndustry));
                //Console.WriteLine("asset value: " + value);
                asset.CompanyAsset.Value = value;
                _assetService.Replace(Guid.Parse(asset.AssetId), _mapper.Map<AssetResource, Asset>(asset));
            }
            return assets;
        }
        private double GrowthRate(double value, double primaryIndustryGrowth, double secondaryIndustryGrowth)
        {
            var backhalf = (7 * primaryIndustryGrowth - 3 * secondaryIndustryGrowth) / 100;
            var growthRate = 1.001 + backhalf;
            //Console.WriteLine("growth rate: " + growthRate);
            return growthRate;
        }
        public async Task<bool> IsRunning(Guid gameId)
        {
            var filter = Builders<EconomyResource>.Filter.Eq("GameId", gameId);
            var incoming = _economy.FindAsync(filter).Result;
            var isRunning = false;
            await incoming.ForEachAsync(g => isRunning = g.isRunning);
            return isRunning;
        }
    }
    public interface IEconService
    {
        Task<string> Motion(Guid econId);
        Task<bool> IsRunning(Guid gameId);
    }
}
