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
    [Route("api/game")]
    [ApiController]
    [Produces("application/json")]
    public class GameController : ControllerBase
    {
        private readonly IMarketService _marketService;
        public bool hasAssets;
        public GameController(IMarketService marketService)
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
        [HttpGet("get-game-status/{gameId:length(36)}")]
        public async Task<ActionResult<bool>> GameStatus(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");

            return Ok(await _marketService.IsRunning(incomingId));
        }
        [HttpGet("get-records/{gameId:length(36)}/{lastSequence}")]
        public ActionResult<ChartPixel> GetRecords(string gameId, int lastSequence)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var output = _marketService.GetRecords(incomingId, lastSequence);
            //Console.WriteLine("records count: " + output.Count);
            return Ok(output);
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
        [HttpPost("on-off/{gameId:length(36)}")]
        public async Task<ActionResult<bool>> OnOff(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            _marketService.StartStop(incomingId);

            return await GameStatus(gameId);
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
