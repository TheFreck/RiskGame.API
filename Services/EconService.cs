using AutoMapper;
using MongoDB.Driver;
using RiskGame.API.Entities;
using RiskGame.API.Logic;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using RiskGame.API.Persistence;
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
        private readonly IAssetService _assetService;
        private readonly IPlayerService _playerService;
        private readonly IShareService _shareService;
        private readonly IEconLogic _econLogic;
        private readonly Random randy;
        private readonly IMapper _mapper;
        private readonly IDatabaseSettings _dbSettings;
        private readonly IMongoCollection<EconomyResource> _economy;
        private readonly IMongoCollection<MarketResource> _market;
        public EconService(IDatabaseSettings settings, IAssetService assetService, IPlayerService playerService, IShareService shareService, IEconLogic econLogic, IMapper mapper, IDatabaseSettings dbSettings)
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
            _econLogic = econLogic;
        }
        public Task<string> Motion(Guid econId)
        {
            var econFilter = Builders<EconomyResource>.Filter.Eq("GameId", econId);
            var economy = _economy.AsQueryable().Where(e => e.GameId == econId).FirstOrDefault();
            var markets = _market.AsQueryable().Where(m => m.GameId == econId).ToArray();
            var assets = _assetService.GetCompanyAssets(econId).Result;
            _market.InsertOne(_mapper.Map<Market, MarketResource>(new Market(econId, assets, randy, null)));
            if (markets.Length == 0) _market.InsertOne(_mapper.Map<Market, MarketResource>(new Market(econId, assets, randy, null)));
            var keepGoing = false;
            do
            {
                var now = DateTime.Now;
                Console.WriteLine($"{now.Minute}:{now.Second}:{now.Millisecond}");
                var loop = new MarketLoopData
                {
                    Filter = econFilter,
                    Economy = economy,
                    KeepGoing = economy.isRunning,
                    EconId = econId
                };
                var next = _econLogic.LoopRound(loop).Result;
                economy = next.Economy;
                keepGoing = next.KeepGoing;
            } while (keepGoing);
            Console.WriteLine("Finito");
            return Task.FromResult("that was fun wasn't it?");
        }

        public async Task<bool> IsRunning(Guid gameId) => await _econLogic.IsRunning(gameId);
        public List<EconomyOut> GetGames() => _mapper.Map<List<EconomyResource>,List<EconomyOut>>(_economy.FindAsync<EconomyResource>(g => true).Result.ToList());
    }
    public interface IEconService
    {
        Task<string> Motion(Guid econId);
        Task<bool> IsRunning(Guid gameId);
        List<EconomyOut> GetGames();
    }
}
