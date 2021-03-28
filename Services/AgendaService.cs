using RiskGame.API.Engine;
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

namespace RiskGame.API.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IAssetService _assetService;
        private readonly Agenda _agenda;
        private readonly Random randy;
        private readonly IMapper _mapper;
        private readonly Asset[] Assets;
        private readonly IMongoCollection<EconomyResource> _economies;
        public bool isRunning { get; set; }
        public bool hasAssets { get; set; }

        public AgendaService(IDatabaseSettings settings, IAssetService assetService, IMapper mapper)
        {
            _mapper = mapper;
            randy = new Random();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            database.DropCollection(settings.EconomyCollectionName);
            _economies = database.GetCollection<EconomyResource>(settings.EconomyCollectionName);
            _assetService = assetService;
            _agenda = new Agenda();
        }
        public async Task<List<EconMetrics>> GetRecords()
        {
            var incomingEcons = await _economies.FindAsync(e => e.SequenceNumber >= 0);
            var econs = new List<EconMetrics>();
            await incomingEcons.ForEachAsync(e => econs.Add(_mapper.Map<EconomyResource,EconMetrics>(e)));
            return econs.OrderBy(e => e.SequenceNumber).ToList();
        }
        public bool AddAssets()
        {
            Console.WriteLine("make assets like they're baybays");
            _agenda.Assets = _assetService.GetCompanyAssetsAsync().Result.ToArray();
            hasAssets = !hasAssets;
            return hasAssets;
        }
        public void Start(int count, int trendiness)
        {
            Motion(count, trendiness);
        }
        public void Stop()
        {
            _agenda.isOn = false;
            isRunning = false;
        }
        public bool IsRunning() => isRunning;
        public void Motion(int count, int trendiness)
        {
            do
            {
                var lastEconomy = new Economy(trendiness, _agenda.Assets, _agenda.Economies.LastOrDefault(), randy);
                var lastEconMetrics = lastEconomy.GetMetrics(GrowAssets(lastEconomy.Assets, lastEconomy));
                var nextEconomy = new Economy(trendiness, _agenda.Assets, lastEconMetrics, randy);
                
                _economies.InsertOne(_mapper.Map<Economy,EconomyResource>(nextEconomy));
                _agenda.Economies.Add(_mapper.Map<Economy,EconMetrics>(nextEconomy));
                count--;
                // process players' turns
            } while (count > 0);
            Console.WriteLine("Finito");
        }
        private CompanyAsset[] GrowAssets(CompanyAsset[] assets, Economy economy)
        {
            foreach (var asset in assets)
            {
                var value = asset.Value * GrowthRate(asset.Value, economy.GetMetric(asset.PrimaryIndustry), economy.GetMetric(asset.SecondaryIndustry));
                asset.Value = value;
            }
            return assets;
        }
        private double GrowthRate(double value, double primaryIndustryGrowth, double secondaryIndustryGrowth) => (1 + primaryIndustryGrowth * Math.Abs(secondaryIndustryGrowth) / 10000);
    }
    public interface IAgendaService
    {
        Task<List<EconMetrics>> GetRecords();
        bool AddAssets();
        void Start(int count, int trendiness);
        public void Stop();
        bool IsRunning();
    }
}
