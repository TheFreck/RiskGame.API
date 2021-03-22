using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB;
using MongoDB.Bson;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models;
using RiskGame.API.Persistence;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.SharesFolder;

namespace RiskGame.API.Controllers
{
    [Route("api/player")]
    [ApiController]
    [Produces("application/json")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IAssetService _assetService;
        private readonly IShareService _shareService;
        private readonly IMapper _mapper;
        public PlayerController(IPlayerService playerService, IAssetService assetService, IShareService shareService, IMapper mapper)
        {
            _playerService = playerService;
            _assetService = assetService;
            _shareService = shareService;
            _mapper = mapper;
        }
        // ***************************************************************
        // GET GET GET GET GET GET GET GET GET GET GET GET GET GET GET GET
        // ***************************************************************
        [HttpGet]
        public async Task<ActionResult<List<Player>>> Get()
        {
            var incoming = _playerService.GetAsync();
            var players = new List<Player>();
            await incoming.Result.ForEachAsync(p => players.Add(_mapper.Map<PlayerResource,Player>(p)));
            return players;
        }
        [HttpGet("{id:length(36)}")]
        public async Task<ActionResult<Player>> Get(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var incoming = _playerService.GetAsync(incomingId);
            var playerRes = new List<PlayerResource>();
            await incoming.Result.ForEachAsync(p => playerRes.Add(p));
            if (playerRes == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<List<PlayerResource>,List<Player>>(playerRes));
        }
        // ****************************************************************
        // POST POST POST POST POST POST POST POST POST POST POST POST POST
        // ****************************************************************
        [HttpPost]
        public async Task<ActionResult<PlayerIn>> Create([FromBody] PlayerIn playerIn)
        {
            var player = _mapper.Map<PlayerIn, Player>(playerIn);
            player.Id = Guid.NewGuid();
            player.PlayerId = player.Id.ToString();
            // check if a cash asset already exists
            var cash = _assetService.GetCash();
            var playerRef = _playerService.ToRef(player);
            var outcome = await _shareService.CreateShares(_mapper.Map<AssetResource, ModelReference>(cash), player.Cash, playerRef, ModelTypes.Cash);
            _playerService.Create(player);
            playerIn.Id = player.Id;
            return Ok(playerIn);
        }
        [HttpPost("add-shares/{playerId:length(36)}/{assetId}/{qty}")]
        // ***************************************************************
        // PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT
        // ***************************************************************
        [HttpPut("{id:length(36)}")]
        public async Task<ActionResult> Update(string id, [FromBody] PlayerIn playerIn)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a proper Guid");

            var incoming = _playerService.GetAsync(incomingId).Result;
            var foundPlayer = new PlayerResource();
            await incoming.ForEachAsync(p => foundPlayer = p);

            if (foundPlayer == null) return NotFound();

            if (playerIn.Name == null) playerIn.Name = foundPlayer.Name;
            if (playerIn.Risk == null) playerIn.Risk = foundPlayer.Risk;
            if (playerIn.Safety == null) playerIn.Safety = foundPlayer.Safety;
            var update = _mapper.Map<PlayerIn, Player>(playerIn);
            update.Id = incomingId;
            update.ObjectId = foundPlayer.ObjectId;
            try
            {
                _playerService.Update(incomingId, _mapper.Map<Player,PlayerResource>(update));
                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
        // **************************************************************
        // DELETE DELETE DELETE DELETE DELETE DELETE DELETE DELETE DELETE
        // **************************************************************
        [HttpDelete("{id:length(36)}")]
        public ActionResult Delete(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var player = _playerService.GetAsync(incomingId);
            if (player == null)
            {
                return NotFound();
            }
            _playerService.Remove(incomingId);
            return NoContent();
        }
    }
}
