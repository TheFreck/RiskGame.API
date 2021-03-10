using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Models.AssetFolder;
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
        private readonly IMapper _mapper;
        public AssetController(AssetService assetService, ShareService shareService, IMapper mapper)
        {
            _shareService = shareService;
            _assetService = assetService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Asset>>> Get() =>
            await _assetService.GetAsync();

        [HttpGet("{id:length(36)}")]
        public async Task<ActionResult<Asset>> Get(string id)
        {
                var isGuid = Guid.TryParse(id, out var incomingId);
                if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
                var asset = await _assetService.GetAsync(incomingId);
                if (asset == null)
                {
                    return NotFound("couldn't find it with that Id");
                }
                return (Asset)asset;
        }
        [HttpGet("shares/{id:length(36)}")]
        public async Task<ActionResult<List<Share>>> GetShares(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var asset = await _assetService.GetAsync(incomingId);
            return _shareService.GetAsync((Asset)asset);
        }

        [HttpPost]
        public ActionResult<AssetIn> Create([FromBody] AssetIn assetIn)
        {
            var asset = _mapper.Map<AssetIn, Asset>(assetIn);
            asset.Id = Guid.NewGuid();
            _assetService.Create(asset, assetIn.SharesOutstanding);
            return _mapper.Map<Asset,AssetIn>(asset);
        }

        [HttpPut("{id:length(36)}")]
        public async Task<IActionResult> Update(string id, AssetIn assetIn)
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
