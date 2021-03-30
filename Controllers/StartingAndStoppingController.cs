using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Entities;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
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
        private readonly IMarketService _marketService;
        public bool hasAssets;
        public StartingAndStoppingController(IMarketService marketService)
        {
            _marketService = marketService;

        }
        // ***
        // GET
        // ***
        [HttpGet]
        public string Get()
        {
            return "got it";
        }
        [HttpGet("new-game")]
        public string NewGame() => _marketService.NewGame().ToString();
        [HttpGet("get-game-status")]
        public ActionResult<bool> GameStatus(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");

            return Ok(_marketService.IsRunning(incomingId));
        }
        [HttpGet("get-records/{gameId:length(36)}")]
        public async Task<ActionResult<List<MarketMetrics>>> GetRecords(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");

            return Ok(await _marketService.GetRecords(incomingId));
        }
        [HttpGet("get-company-assets")]
        public ActionResult<List<CompanyAsset>> GetCompanyAssets(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");

            return _marketService.GetCompanyAssets(incomingId);
        }

        // ****
        // POST
        // ****
        [HttpPost]
        public ActionResult<string> OnOff([FromBody]string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            _marketService.StartStop(incomingId);
            return Ok("Started//Stopped game " + gameId);
        }

        // ***
        // PUT
        // ***
        [HttpPut("pixel-trend/{pixel}/{trend}")]
        public string SetPixelAndTrend(Guid gameId, int pixel, int trend)
        {
            _marketService.SetPixelCount(gameId, pixel);
            _marketService.SetTrendiness(gameId, trend);
            return "dun did it";
        }

        // ******
        // DELETE
        // ******

    }
}
