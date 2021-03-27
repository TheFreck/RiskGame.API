using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Engine;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Controllers
{
    [Route("api/starting-and-stopping")]
    [ApiController]
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

        // ****
        // POST
        // ****
        [HttpPost]
        public void OnOrOff()
        {
            if (!_agendaService.AddAssets())
            {
                if (_agendaService.IsRunning()) _agendaService.Stop();
                else _agendaService.Start(100, 8);
            }
        }
    }
}
