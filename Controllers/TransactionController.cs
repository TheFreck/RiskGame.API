using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Entities;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models.TransactionFolder;
using RiskGame.API.Persistence;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    [Produces("application/json")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IAssetService _assetService;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionService transactionService, IAssetService assetService, IMapper mapper)
        {
            _transactionService = transactionService;
            _assetService = assetService;
            _mapper = mapper;
        }
        // ***************************************************************
        // GET GET GET GET GET GET GET GET GET GET GET GET GET GET GET GET
        // ***************************************************************
        [HttpGet]
        public ActionResult<string> Get() => Ok("you are connected to the transactions controller");
        [HttpGet("transactions")]
        public List<TransactionResource> GetTransactions(Guid gameId) => _transactionService.GetTransactions(gameId);
        [HttpGet("get-trades/{gameId:length(36)}/{assetId}/{since}")]
        public ActionResult<List<ChartPixel>> GetTrades(string gameId, string assetId, DateTime since)
        {
            var isGameGuid = Guid.TryParse(gameId, out var game);
            if (!isGameGuid) return NotFound("Me thinks that Id was not a Guid");
            var isAssetGuid = Guid.TryParse(assetId, out var asset);
            if (!isAssetGuid) return NotFound("Me thinks that Id was not a Guid");

            return _assetService.GetTrades(game, asset, since);
        }
        // ****************************************************************
        // POST POST POST POST POST POST POST POST POST POST POST POST POST
        // ****************************************************************
        [HttpPost("trade")]
        public ActionResult<TradeClient> Trade([FromBody]TradeClient trade)
        {
            try
            {
                _transactionService.InsertTrade(_mapper.Map<TradeClient,TransactionResource>(trade));
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
        // ***************************************************************
        // PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT
        // ***************************************************************

        // **************************************************************
        // DELETE DELETE DELETE DELETE DELETE DELETE DELETE DELETE DELETE 
        // **************************************************************
    }
}
