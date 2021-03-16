using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Controllers
{
    [Route("api/asset")]
    [ApiController]
    [Produces("application/json")]
    public class AssetController : ControllerBase
    {
        private readonly AssetService _assetService;
        private readonly ShareService _shareService;
        private readonly PlayerService _playerService;
        private readonly IMapper _mapper;
        public AssetController(AssetService assetService, ShareService shareService, PlayerService playerService, IMapper mapper)
        {
            _shareService = shareService;
            _assetService = assetService;
            _playerService = playerService;
            _mapper = mapper;
        }
        // ***************************************************************
        // GET GET GET GET GET GET GET GET GET GET GET GET GET GET GET GET
        // ***************************************************************
        [HttpGet]
        public async Task<ActionResult<List<Asset>>> Get() =>
            await _assetService.GetAsync();

        [HttpGet("{id:length(36)}")]
        public async Task<ActionResult<Asset>> Get(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var incoming = await _assetService.GetAsync(incomingId);
            var asset = new Asset();
            await incoming.ForEachAsync(a => asset = a);
            if (asset.Id == Guid.Empty) NotFound("couldn't find it with that Id");
            return asset;
        }
        [HttpGet("shares/{id:length(36)}")]
        public async Task<ActionResult<List<Asset>>> GetShares(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var incomingAsset = await _assetService.GetAsync(incomingId);
            var asset = new Asset();
            await incomingAsset.ForEachAsync(a => asset = a);
            if (asset.Id == Guid.Empty)
            {
                return NotFound("couldn't find it with that Id");
            }
            var incomingShares = _shareService.GetAsync(asset);
            var shares = new List<Share>();
            await incomingShares.Result.ForEachAsync(s => shares.Add(s));
            var returnShares = new List<Share>();
            foreach(var share in shares)
            {
                returnShares.Add(share);
                Console.WriteLine(share.Name);
                Console.WriteLine(share.Id);
            }
            return Ok(returnShares);
        }
        // ****************************************************************
        // POST POST POST POST POST POST POST POST POST POST POST POST POST
        // ****************************************************************
        [HttpPost]
        public async Task<ActionResult<AssetIn>> Create([FromBody]AssetIn assetIn)
        {
            var asset = new Asset();
            asset = _mapper.Map<AssetIn, Asset>(assetIn);
            asset.Id = Guid.NewGuid();
            var assetId = _assetService.Create(asset);
            var theShares = await _shareService.CreateShares(_mapper.Map<Asset, ModelReference>(asset), asset.SharesOutstanding, new ModelReference(),ModelTypes.Share);
            var haus = _playerService.HAUS;
            haus.Portfolio.AddRange(theShares);
            var hausUpdate = new Player(haus.Id) { Portfolio = haus.Portfolio };
            var hausResult = await _playerService.UpdateHaus(hausUpdate);
            if(hausResult.Name == "HAUS") return _mapper.Map<Asset, AssetIn>(asset);
            else return NotFound(hausResult.Message);
        }
        [HttpPost("add-shares/{id:length(36)}/{qty}")]
        public async Task<ActionResult<List<ModelReference>>> AddShares(string id, int qty)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");

            var incoming = await _assetService.GetAsync(incomingId);
            var asset = new ModelReference();
            await incoming.ForEachAsync(a => asset = _mapper.Map<Asset,ModelReference>(a));
            if (asset.Id == Guid.Empty) NotFound("couldn't find it with that Id");
            return await _shareService.CreateShares(asset, qty, new ModelReference(), ModelTypes.Share);
        }
        // ***************************************************************
        // PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT
        // ***************************************************************
        [HttpPut("{id:length(36)}")]
        public async Task<IActionResult> Update(string id, [FromBody]AssetIn assetIn)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var awaitedAsset = await _assetService.GetAsync(incomingId);
            Asset foundAsset = (Asset)awaitedAsset;
            if (foundAsset == null) return NotFound();

            if (assetIn.Name == null) assetIn.Name = foundAsset.Name;
            if (assetIn.BookValue == null) assetIn.BookValue = foundAsset.BookValue;
            if (assetIn.RateOfReturn == null) assetIn.RateOfReturn = foundAsset.RateOfReturn;

            var update = _mapper.Map<AssetIn, Asset>(assetIn);
            update.Id = incomingId;
            try
            {
                _assetService.Update(incomingId, update);
                return NoContent();
            }
            catch (Exception e)
            {
                return NotFound(e);
            }
        }
        // **************************************************************
        // DELETE DELETE DELETE DELETE DELETE DELETE DELETE DELETE DELETE 
        // **************************************************************
        [HttpDelete("{id:length(36)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var asset = await _assetService.GetAsync(incomingId);
            if (asset == null)
            {
                return NotFound();
            }
            _assetService.Remove(incomingId);
            return NoContent();
        }
    }
}
