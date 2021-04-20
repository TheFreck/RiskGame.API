using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Entities;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RiskGame.API.Models.AssetFolder;

namespace RiskGame.API.Controllers
{
    [Route("api/game")]
    [ApiController]
    [Produces("application/json")]
    public class GameController : ControllerBase
    {
        private readonly IMarketService _marketService;
        private readonly IEconService _econService;
        public bool hasAssets;
        public GameController(IMarketService marketService, IEconService econService)
        {
            _marketService = marketService;
            _econService = econService;

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
        public ActionResult<bool> GameStatus(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");

            return Ok(_econService.IsRunning(incomingId));
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
        public ActionResult<CompanyAsset[]> GetCompanyAssets(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");

            return _marketService.GetCompanyAssets(incomingId);
        }
        [HttpGet("get-games")]
        public ActionResult<List<EconomyOut>> GetGames() => _econService.GetGames();
        [HttpGet("get-markets")]
        public ActionResult<List<MarketMetrics>> GetMarkets() => _marketService.GetMarkets();

        // ****
        // POST
        // ****
        [HttpPost("on-off/{gameId:length(36)}/{isRunning}")]
        public void OnOff(string gameId, bool isRunning)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            _marketService.StartStop(incomingId, isRunning);
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
        [HttpPut("blowup-the-inside-world")]
        public ActionResult<string> BlowUpTheInsideWorld([FromBody] string secretCode)
        {
            Console.WriteLine("secret code: " + secretCode);
            _marketService.BigBang(secretCode);
            return Ok("sweet oblivion!");
        }

        // ******
        // DELETE
        // ******
        [HttpDelete("end-game/{gameId:length(36)}")]
        public ActionResult<string> EndGame(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            return Ok(_marketService.EndGame(incomingId));
        }
    }
}
