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
        public ActionResult<List<Asset>> Get() =>
            _assetService.Get();

        [HttpGet("{id:length(36)}")]
        public ActionResult<Asset> Get(string id)
        {
                var isGuid = Guid.TryParse(id, out var incomingId);
                if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
                var asset = _assetService.Get(incomingId);
                if (asset == null)
                {
                    return NotFound("couldn't find it with that Id");
                }
                return asset;
        }
        [HttpGet("shares/{id:length(36)}")]
        public ActionResult<List<Share>> GetShares(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var asset = _assetService.Get(incomingId);
            return _shareService.Get(asset);
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
        public IActionResult Update(string id, AssetIn assetIn)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var foundAsset = _assetService.Get(incomingId);
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
        public IActionResult Delete(string id)
        {
            var isGuid = Guid.TryParse(id, out var incomingId);
            if (!isGuid) return NotFound("Me thinks that Id was not a Guid");
            var asset = _assetService.Get(incomingId);
            if (asset == null)
            {
                return NotFound();
            }
            _assetService.Remove(incomingId);
            return NoContent();
        }
    }
}
