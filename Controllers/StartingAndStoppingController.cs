using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Engine;
using RiskGame.API.Entities;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Controllers
{
    [Route("api/starting-and-stopping")]
    [ApiController]
    [Produces("application/json")]
    public class StartingAndStoppingController : ControllerBase
    {
        private readonly IAgendaService _agendaService;
        public bool hasAssets;
        public StartingAndStoppingController(IAgendaService agendaService)
        {
            _agendaService = agendaService;
        }
        // ***
        // GET
        // ***
        [HttpGet]
        public string Get() => "you made it";
        [HttpGet("get-game-status")]
        public bool GameStatus() => _agendaService.IsRunning();
        [HttpGet("get-records")]
        public async Task<List<EconMetrics>> GetRecords() => await _agendaService.GetRecords();
        [HttpGet("next/{frames}/{trendiness}")]
        public List<EconMetrics> Next(int frames, int trendiness) => _agendaService.Motion(frames, trendiness);
        [HttpGet("add-assets")]
        public void LoadAssets() => _agendaService.LoadAssets();
        [HttpGet("get-company-assets")]
        public List<CompanyAsset> GetCompanyAssets()
        {
            return _agendaService.GetCompanyAssets();
        }
        // ****
        // POST
        // ****
        [HttpPost]
        public string OnOrOff()
        {
            var result = "";
            if (_agendaService.IsRunning())
            {
                result = "off";
                _agendaService.Stop();
            }
            else
            {
                result = "on and running";
                _agendaService.Start(100, 8);
            }
            return result;
        }
    }
}
