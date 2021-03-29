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
        private CompanyAsset[] Assets;
        private readonly IMongoCollection<EconomyResource> _economies;
        public bool isRunning { get; set; }

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
            Assets = _assetService.GetCompanyAssets().Result.ToArray();
        }
        public async Task<List<EconMetrics>> GetRecords()
        {
            var incomingEcons = await _economies.FindAsync(e => e.SequenceNumber >= 0);
            var econs = new List<EconMetrics>();
            await incomingEcons.ForEachAsync(e => econs.Add(_mapper.Map<EconomyResource,EconMetrics>(e)));
            return econs.OrderBy(e => e.SequenceNumber).ToList();
        }
        public void LoadAssets()
        {
            Console.WriteLine("make assets like they're baybays");
            Assets = _assetService.GetCompanyAssets().Result.ToArray();
            _agenda.Assets = Assets;
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
        public List<EconMetrics> Motion(int count, int trendiness)
        {
            LoadAssets();
            var econList = new List<EconMetrics>();
            do
            {
                // check assets
                var lastEconomy = new Economy(trendiness, Assets, _agenda.Economies.LastOrDefault(), randy);
                var lastEconMetrics = lastEconomy.GetMetrics(GrowAssets(lastEconomy.Assets, lastEconomy));
                var nextEconomy = new Economy(trendiness, _agenda.Assets, lastEconMetrics, randy);
                var nextEconMetrics = nextEconomy.GetMetrics(GrowAssets(nextEconomy.Assets, nextEconomy));
                econList.Add(nextEconMetrics);
                Assets = nextEconMetrics.Assets;
                _economies.InsertOne(_mapper.Map<Economy,EconomyResource>(nextEconomy));
                _agenda.Economies.Add(_mapper.Map<Economy,EconMetrics>(nextEconomy));
                count--;

                // process players' turns
            } while (count > 0);
            Console.WriteLine("Finito");
            return econList;
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
        public List<CompanyAsset> GetCompanyAssets()
        {
            return _assetService.GetCompanyAssets().Result;
        }
    }
    public interface IAgendaService
    {
        Task<List<EconMetrics>> GetRecords();
        public void LoadAssets();
        void Start(int count, int trendiness);
        public void Stop();
        bool IsRunning();
        List<EconMetrics> Motion(int count, int trendiness);
        public List<CompanyAsset> GetCompanyAssets();
    }
}
