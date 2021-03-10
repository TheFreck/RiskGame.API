using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Entities;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
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
        private readonly TransactionService _transactionService;
        private readonly IMapper _mapper;

        public TransactionController(TransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "you are connected to the transactions controller";
        }
        [HttpPost]
        public ActionResult<TradeTicket> Trade(TradeTicket trade)
        {
            return _transactionService.Transact(trade);
        }
    }
}
