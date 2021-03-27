using RiskGame.API.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Entities;

namespace RiskGame.API.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IAssetService _assetService;
        private readonly Agenda _agenda;
        public bool isRunning { get; set; }
        public bool hasAssets { get; set; }
        public AgendaService(IAssetService assetService)
        {
            _assetService = assetService;
            _agenda = new Agenda(_assetService);
        }
        public bool AddAssets()
        {
            Console.WriteLine("make assets like they're baybays");
            hasAssets = !hasAssets;
            return hasAssets;
        }
        public void Start(int count, int trendiness)
        {
            _agenda.Motion(count, trendiness);
        }
        public void Stop()
        {
            _agenda.isOn = false;
            isRunning = false;
        }
        public bool IsRunning() => isRunning;
    }
    public interface IAgendaService
    {
        bool AddAssets();
        void Start(int count, int trendiness);
        public void Stop();
        bool IsRunning();
    }
}
