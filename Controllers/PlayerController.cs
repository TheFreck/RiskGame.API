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
        private readonly PlayerService _playerService;
        private readonly AssetService _assetService;
        private readonly ShareService _shareService;
        private readonly IMapper _mapper;
        public PlayerController(PlayerService playerService, AssetService assetService, ShareService shareService, IMapper mapper)
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
            await incoming.Result.ForEachAsync(p => players.Add(p));
            return players;
        }

        [HttpGet("{id:length(36)}")]
        public async Task<ActionResult<Player>> Get(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var incoming = _playerService.GetAsync(incomingId);
            var player = new Player();
            await incoming.Result.ForEachAsync(p => player = p);
            if(player == null)
            {
                return NotFound();
            }
            return player;
        }
        // ****************************************************************
        // POST POST POST POST POST POST POST POST POST POST POST POST POST
        // ****************************************************************
        [HttpPost]
        public async Task<ActionResult<PlayerIn>> Create([FromBody]PlayerIn playerIn)
        {
            var player = _mapper.Map<PlayerIn, Player>(playerIn);
            player.Id = Guid.NewGuid();
            // check if a cash asset already exists
            var incoming = _assetService.GetAsync(Guid.Parse("5490B3E5-1242-4EB9-A9B3-627878795996")).Result;
            var cash = new Asset();
            await incoming.ForEachAsync(c => cash = c);
            var cashShares = new List<Share>();
            var playerRef = _mapper.Map<Player, ModelReference>(player);
            if(cash.SharesOutstanding == 0)
            {
                // if cash does not exist then create it
                cash.Id = Guid.Parse("5490B3E5-1242-4EB9-A9B3-627878795996");
                cash.Name = "Cash";
                cash.SharesOutstanding += playerIn.Cash;
                var outcome = _assetService.Create(cash);
                var incomingCash = _shareService.CreateShares(_mapper.Map<Asset,ModelReference>(cash),player.Cash, playerRef).Result;
                incomingCash.ForEach(c => cashShares.Add(c));
                foreach(var cShare in cashShares)
                {
                    cShare.CurrentOwner = _mapper.Map<Player, ModelReference>(player);
                    player.Wallet.Add(_mapper.Map<Share, ModelReference>(cShare));
                }
            }
            else
            {
                var outcome = await _shareService.CreateShares(_mapper.Map<Asset,ModelReference>(cash), player.Cash, playerRef);

            }

            _playerService.Create(player);
            return playerIn;
        }
        [HttpPost("add-shares/{playerId:length(36)}/{assetId}/{qty}")]
        //
        // Adds shares of assets or cash to the player's portfolio/wallet respectively
        public async Task<ActionResult<Player>> AddShares(string playerId, string assetId, int qty)
        {
            // get the player
            var isPlayerGuid = Guid.TryParse(playerId, out var incomingPlayerId);
            if (!isPlayerGuid) return NotFound("Me thinks that Player Id was not a Guid");
            var incomingPlayer = _playerService.GetAsync(incomingPlayerId);
            var player = new Player();
            await incomingPlayer.Result.ForEachAsync(p => player = p);

            // get the asset
            var isAssetGuid = Guid.TryParse(assetId, out var incomingAssetId);
            if (!isAssetGuid) return NotFound("Me thinks that Asset Id was not a Guid");
            var incomingAsset = _assetService.GetAsync(incomingAssetId);
            var asset = new Asset();
            await incomingAsset.Result.ForEachAsync(a => asset = a);

            // get the shares
            var incomingShares = _shareService.GetAsync(asset.Id);
            var shares = new List<Share>();
            await incomingShares.Result.ForEachAsync(s => shares.Add(s));

            // add shares to player's portfolio
            foreach(var share in shares)
            {
                if (qty == 0) break;
                if(share.CurrentOwner == null)
                {
                    player.Portfolio.Add(_mapper.Map<Share, ModelReference>(share));
                    qty--;
                }
            }

            try
            {
                // update player
                _playerService.Update(player.Id, player);
                return player;
            }
            catch (Exception e)
            {
                return new Player
                {
                    Name = "Sumpin went wrong: " + e.Message
                };
            }
        }

        // ***************************************************************
        // PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT
        // ***************************************************************
        [HttpPut("{id:length(36)}")]
        public async Task<ActionResult> Update(string id, [FromBody]PlayerIn playerIn)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a proper Guid");

            var incoming = _playerService.GetAsync(incomingId);
            var foundPlayer = new Player();
            await incoming.Result.ForEachAsync(p => foundPlayer = p);

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
            if (playerIn.Wallet.Count() == 0) update.Wallet = foundPlayer.Wallet;
            else
            {
                foreach(var item in playerIn.Wallet)
                {
                    update.Wallet.Add(item);
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
        [HttpPut("clear-portfolio/{id:length(36)}")]
        public async Task<ActionResult<string>> ClearPortfolio(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a proper Guid");
            return await _playerService.EmptyPortfolio(incomingId);
        }
        [HttpPut("clear-cash/{id:length(36)}")]
        public async Task<ActionResult<string>> ClearCash(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a proper Guid");
            return await _playerService.EmptyWallet(incomingId);
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
            if(player == null)
            {
                return NotFound();
            }
            _playerService.Remove(incomingId);
            return NoContent();
        }
    }
}
