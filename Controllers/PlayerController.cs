﻿using AutoMapper;
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
using RiskGame.API.Entities.Enums;

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
            await incoming.Result.ForEachAsync(p => players.Add(_mapper.Map<PlayerResource, Player>(p)));
            return players;
        }
        [HttpGet("{gameId:length(36)}")]
        public async Task<ActionResult<Player>> Get(string gameId)
        {
            var isGuid = Guid.TryParse(gameId, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var incoming = _playerService.GetPlayerAsync(incomingId);
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
        [HttpPost("new-player")]
        public async Task<ActionResult<PlayerIn>> Create([FromBody] PlayerIn playerIn)
        {
            var player = _mapper.Map<PlayerIn, Player>(playerIn);
            player.Id = Guid.NewGuid();
            player.PlayerId = player.Id.ToString();
            
            var incomingCash = _assetService.GetCashAsync(player.GameId).Result;
            var cash = new AssetResource();
            await incomingCash.ForEachAsync(c => cash = c);
            var playerRef = _playerService.ToRef(player);
            var outcome = await _shareService.CreateShares(_mapper.Map<AssetResource, ModelReference>(cash), player.Cash, playerRef, ModelTypes.Cash);
            cash.SharesOutstanding += player.Cash;
            try
            {
                _assetService.Replace(Guid.Parse(cash.AssetId), _mapper.Map<AssetResource, Asset>(cash));
                _playerService.Create(player);
                playerIn.GameId = player.GameId;
                playerIn.Id = player.Id;
                return Ok(playerIn);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
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

            var incoming = _playerService.GetPlayerAsync(incomingId).Result;
            var foundPlayer = new PlayerResource();
            await incoming.ForEachAsync(p => foundPlayer = p);

            if (foundPlayer == null) return NotFound();

            if (playerIn.Name == null) playerIn.Name = foundPlayer.Name;
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
            var player = _playerService.GetPlayerAsync(incomingId);
            if (player == null)
            {
                return NotFound();
            }
            _playerService.Remove(incomingId);
            return NoContent();
        }

        //[HttpDelete("game/start/initialize/{secretPassword}")]
        //public ActionResult<string> Initialize(string secretPassword)
        //{
        //    if (secretPassword == "Playa101") return Ok(_playerService.Initialize());
        //    else return Unauthorized("no way doood!");
        //}
    }
}
