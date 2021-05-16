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
using AutoMapper;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models;
using System.Diagnostics;
using System.Threading;

namespace RiskGame.API.Controllers
{
    [Route("api/game")]
    [ApiController]
    [Produces("application/json")]
    public class GameController : ControllerBase
    {
        private readonly IMarketService _marketService;
        private readonly IEconService _econService;
        private readonly IAssetService _assetService;
        private readonly IPlayerService _playerService;
        private readonly IShareService _shareService;
        private readonly IMapper _mapper;
        public bool hasAssets;
        public GameController(IMarketService marketService, IEconService econService, IAssetService assetService, IPlayerService playerService, IShareService shareService, IMapper mapper)
        {
            _marketService = marketService;
            _econService = econService;
            _assetService = assetService;
            _playerService = playerService;
            _shareService = shareService;
            _mapper = mapper;
        }
        // ***
        // GET
        // ***
        [HttpGet]
        public string Get()
        {
            return "got it";
        }
        [HttpGet("new-game/{assetQty}")]
        public async Task<ActionResult<EconomyOut>> NewGame(int assetQty)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("starting the timer");
            var gameId = Guid.NewGuid();
            var haus = _playerService.CreateOne(new Player("HAUS", gameId));
            var assets = new List<AssetResource>();
            var sharesIssued = 4444;
            for (var i = 0; i < assetQty; i++)
            {
                var asset = _mapper.Map<Asset, AssetResource>(new Asset
                {
                    Name = $"Asset_{i}",
                    GameId = gameId,
                    SharesOutstanding = sharesIssued,
                    TradeHistory = new List<Tuple<TradeType, decimal>>(),
                    CompanyHistory = new List<Tuple<DateTime, decimal>>(),
                    AssetId = Guid.NewGuid(),
                    Debt = new Random().Next(1, 10),
                    CompanyAsset = new CompanyAsset()
                });

                // add the IPO trade after creation to allow company asset to be created
                asset.CompanyAssetValuePerShare = asset.CompanyAsset.Value * asset.Debt / asset.SharesOutstanding;
                asset.TradeHistory.Add(Tuple.Create(TradeType.Buy, asset.CompanyAssetValuePerShare));
                asset.MostRecentValue = asset.CompanyAsset.Value * asset.Debt;
                var outcome = await _assetService.Create(asset);
                if (outcome == "done") _shareService.CreateShares(_mapper.Map<AssetResource, ModelReference>(asset), asset.SharesOutstanding, _mapper.Map<PlayerResource,ModelReference>(haus), asset.ModelType, gameId);
                assets.Add(asset);
            }
            var counter = 500;
            do
            {
                counter--;
                Thread.Sleep(250);
            } while (_shareService.GetQueryableGameShares(gameId).Count() != sharesIssued * assetQty && counter > 0);
            stopwatch.Stop();
            Console.WriteLine("elapsed milliseconds: " + stopwatch.ElapsedMilliseconds);
            if (counter <= 0) return NoContent();
            return Ok(new EconomyOut { GameId = _marketService.NewGame(gameId, assets.ToArray()), Assets = assets.Select(a => a.AssetId).ToArray()});
        }
        [HttpGet("get-game-status/{gameId:length(36)}")]
        public ActionResult<bool> GameStatus(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            try
            {
                return Ok(_econService.IsRunning(incomingId));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
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
        [HttpGet("copy")]
        public ActionResult CopyData()
        {
            try
            {
                _assetService.CopyData();
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e);
            }
        }
        // ****
        // POST
        // ****
        [HttpPost("on-off/{gameId:length(36)}/{isRunning}")]
        public void AssetsOnOff(string gameId, bool isRunning)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            _marketService.AssetsStartStop(incomingId, isRunning);
            Console.WriteLine("after start stop");
        }
        [HttpPost("trading-on-off/{gameId:length(36)}/{isRunning}")]
        public void TradingOnOff(string gameId, bool isRunning)
        {
            Thread.Sleep(5000);
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            _playerService.TradingStartStop(incomingId, isRunning);
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
            return Ok(_marketService.BigBang(secretCode));
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
