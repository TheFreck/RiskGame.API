using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Controllers
{
    [Route("api/player")]
    [ApiController]
    [Produces("application/json")]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;
        private readonly IMapper _mapper;
        public PlayerController(PlayerService playerService, IMapper mapper)
        {
            _playerService = playerService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<Player>> Get() =>
            _playerService.Get();

        [HttpGet("{id:length(36)}")]
        public ActionResult<Player> Get(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var player = _playerService.Get(incomingId);
            if(player == null)
            {
                return NotFound();
            }
            return player;
        }

        [HttpPost]
        public ActionResult<PlayerIn> Create([FromBody]PlayerIn playerIn)
        {
            var player = _mapper.Map<PlayerIn, Player>(playerIn);
            player.Id = Guid.NewGuid();
            _playerService.Create(player);
            return playerIn;
        }

        [HttpPut("{id:length(36)}")]
        public IActionResult Update(string id, [FromBody]PlayerIn playerIn)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a proper Guid");

            var foundPlayer = _playerService.Get(incomingId);
            if (foundPlayer == null) return NotFound();

            if (playerIn.Name == null) playerIn.Name = foundPlayer.Name;
            if (playerIn.Risk == null) playerIn.Risk = foundPlayer.Risk;
            if (playerIn.Safety == null) playerIn.Safety = foundPlayer.Safety;
            var update = _mapper.Map<PlayerIn, Player>(playerIn);
            // if the player does not have a current portfolio then set the player portfolio to the one passed in
            if (playerIn.Portfolio == null) update.Portfolio = foundPlayer.Portfolio;
            // otherwise update player's portfolio with the new info
            else
            {
                foreach (var item in playerIn.Portfolio)
                {
                    update.Portfolio.Add(item);
                }
            }
            if (playerIn.Cash == null) update.Cash = foundPlayer.Cash;
            else
            {
                foreach(var item in playerIn.Cash)
                {
                    update.Cash.Add(item);
                }
            }


            update.Id = incomingId;
            update.ObjectId = foundPlayer.ObjectId;
            try
            {
                _playerService.Update(incomingId, update);
                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("{id:length(36)}")]
        public IActionResult Delete(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var player = _playerService.Get(incomingId);
            if(player == null)
            {
                return NotFound();
            }
            _playerService.Remove(incomingId);
            return NoContent();
        }
    }
}
