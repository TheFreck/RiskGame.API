using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
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
        private readonly IAssetService _assetService;
        private readonly IShareService _shareService;
        private readonly IPlayerService _playerService;
        private readonly IMapper _mapper;
        public AssetController(IAssetService assetService, IShareService shareService, IPlayerService playerService, IMapper mapper)
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
        public async Task<List<Asset>> Get() =>
            _mapper.Map<List<AssetResource>, List<Asset>>(await _assetService.GetAsync());
        [HttpGet("{id:length(36)}")]
        public async Task<ActionResult<Asset>> Get(string id) // assetId
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var incoming = await _assetService.GetAsync(incomingId);
            var asset = new AssetResource();
            await incoming.ForEachAsync(a => asset = a);
            if (asset.AssetId == Guid.Empty.ToString()) NotFound("couldn't find it with that Id");
            return _mapper.Map<AssetResource, Asset>(asset);
        }
        [HttpGet("cash")]
        public ActionResult<AssetWithShares> GetCash()
        {
            var cash = _assetService.GetCash();
            var cashShares = _shareService.GetAssetSharesAsync(cash).Result;
            return Ok(new AssetWithShares
            {
                Asset = _mapper.Map<AssetResource, Asset>(cash),
                Shares = _mapper.Map<List<ShareResource>, List<Share>>(cashShares)
            });
        }
        [HttpGet("shares/{id:length(36)}")]
        public async Task<ActionResult<AssetWithShares>> GetShares(string id) // assetId
        {
            var asset = new AssetResource();
            // Find asset
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var incomingAsset = await _assetService.GetAsync(incomingId);
            await incomingAsset.ForEachAsync(a => asset = a);
            if (asset.AssetId == Guid.Empty.ToString()) return NotFound("couldn't find it with that Id");
            // Get asset shares
            var shares = await _shareService.GetAssetSharesAsync(asset);
            // Make sure the shares have Id's
            var returnShares = new List<Share>();
            foreach (var share in shares)
            {
                var updateShare = _mapper.Map<ShareResource, Share>(share);
                updateShare.Id = Guid.Parse(updateShare.ShareId);
                returnShares.Add(updateShare);
            }
            return Ok(new AssetWithShares
            {
                Asset = _mapper.Map<AssetResource, Asset>(asset),
                Shares = returnShares
            });
        }
        [HttpGet("player-shares/{id:length(36)}/{type}/{qty}")]
        public async Task<ActionResult<List<ModelReference>>> GetPlayerShares(string id, int type, int qty) // playerId
        {
            Console.WriteLine("Get shares: " + type);
            var modelType = (ModelTypes)type;
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var incomingPlayer = await _playerService.GetAsync(incomingId);
            var playerRes = new PlayerResource();
            await incomingPlayer.ForEachAsync(p => playerRes = p);
            var playerRef = _playerService.ResToRef(playerRes);
            var shareType = await _shareService.GetPlayerShares(playerRef,modelType);

            var allShares = _mapper.Map<List<ShareResource>, List<ModelReference>>(shareType);
            var allOrNone = 0;
            if (qty == 0) allOrNone = allShares.Count;
            else allOrNone = qty;
            var output = allShares.Take(allOrNone).Where(s => s.ModelType == modelType);
            return Ok(output);
        }
        // ****************************************************************
        // POST POST POST POST POST POST POST POST POST POST POST POST POST
        // ****************************************************************
        [HttpPost]
        public async Task<ActionResult<AssetIn>> Create([FromBody] AssetIn assetIn)
        {
            // Income variables
            var L = 5.2;
            var k = -.4;
            var x0 = 10;

            // Build Asset
            var randy = new Random();
            var asset = _mapper.Map<AssetIn, Asset>(assetIn);
            asset.Id = Guid.NewGuid();
            asset.AssetId = asset.Id.ToString();
            asset.CompanyAsset = new CompanyAsset
            {
                PrimaryIndustry = (IndustryTypes)randy.Next(5),
                SecondaryIndustry = (IndustryTypes)randy.Next(5),
                Value = asset.SharesOutstanding
            };

            // Create Asset
            var assetId = _assetService.Create(asset);
            var hausRef = _playerService.GetHAUSRef();

            // Create Shares
            var theShares = await _shareService.CreateShares(_mapper.Map<Asset, ModelReference>(asset), asset.SharesOutstanding, hausRef, ModelTypes.Share);
            var cashAsset = _assetService.GetCash();
            var cashShares = await _shareService.CreateShares(_mapper.Map<AssetResource, ModelReference>(cashAsset), asset.SharesOutstanding, hausRef, ModelTypes.Cash);
            return _mapper.Map<Asset, AssetIn>(asset);
        }
        [HttpPost("add-shares/{id:length(36)}/{qty}/{type}")]
        public async Task<ActionResult<List<ModelReference>>> AddShares(string id, int qty, int type) // assetId, number of shares
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var haus = _playerService.GetHAUS();
            var incoming = await _assetService.GetAsync(incomingId);
            var asset = new ModelReference();
            await incoming.ForEachAsync(a => asset = _mapper.Map<AssetResource, ModelReference>(a));
            if (asset.Id == Guid.Empty) NotFound("couldn't find it with that Id");
            return Ok(await _shareService.CreateShares(asset, qty, _playerService.ToRef(haus), (ModelTypes)type));
        }
        // ***************************************************************
        // PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT PUT
        // ***************************************************************
        [HttpPut("{id:length(36)}")]
        public async Task<IActionResult> Update(string id, [FromBody] AssetIn assetIn) // assetId, changeSet
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var awaitedAsset = await _assetService.GetAsync(incomingId);
            Asset foundAsset = (Asset)awaitedAsset;
            if (foundAsset == null) return NotFound();

            if (assetIn.Name == null) assetIn.Name = foundAsset.Name;

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
        public async Task<IActionResult> Delete(string id) // assetId
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

        [HttpDelete("game/start/initialize/{secretPassword}")]
        public ActionResult<string> Initialize(string secretPassword)
        {
            if (secretPassword == "Playa101") return Ok(_assetService.Initialize());
            else return Unauthorized("no way doood!");
        }
    }
}
