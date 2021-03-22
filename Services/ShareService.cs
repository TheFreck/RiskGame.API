using MongoDB.Bson;
using MongoDB.Driver;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models.PlayerFolder;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace RiskGame.API.Services
{
    public class ShareService : IShareService
    {
        private readonly IMongoCollection<ShareResource> _shares;
        private readonly IMapper _mapper;
        public ShareService(IDatabaseSettings settings, IMapper mapper)
        {
            _mapper = mapper;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _shares = database.GetCollection<ShareResource>(settings.ShareCollectionName);
        }
        public async Task<IAsyncCursor<ShareResource>> GetAsync() => await _shares.FindAsync(share => true);
        public async Task<List<ShareResource>> GetAssetSharesAsync(AssetResource asset)
        {
            var filter = Builders<ShareResource>.Filter.Eq("_assetId", Guid.Parse(asset.AssetId));
            var incomingShares = _shares.FindAsync(filter).Result;
            var shares = new List<ShareResource>();
            await incomingShares.ForEachAsync(s => shares.Add(s));
            return shares;
        }
        public async Task<IAsyncCursor<ShareResource>> GetAsync(Guid id)
        {
            var filter = Builders<ShareResource>.Filter.Eq("Id", id.ToString());
            return await _shares.FindAsync(filter);
        }
        public async Task<List<ShareResource>> GetAsync(List<Guid> shares)
        {
            var stringShares = new List<string>();
            shares.ForEach(s => stringShares.Add(s.ToString()));
            var filter = Builders<ShareResource>.Filter.In(s => s.ShareId, stringShares);
            var incoming = await _shares.FindAsync<ShareResource>(filter);
            var returnShares = new List<ShareResource>();
            await incoming.ForEachAsync(s => returnShares.Add(s));
            return returnShares;
        }
        public async Task<IAsyncCursor<ShareResource>> GetNonCashAsync() => await _shares.FindAsync(s => s.ModelType.Equals(ModelTypes.Share));
        public async Task<List<ShareResource>> GetPlayerShares(ModelReference playerRef)
        {
            var filter = Builders<ShareResource>.Filter.Eq(p => p.CurrentOwner.Id, playerRef.Id) &
            Builders<ShareResource>.Filter.Eq(p => p.ModelType, ModelTypes.Share);
            var incomingShares = await _shares.FindAsync(filter);
            var yourSharesSirOrMadam = new List<ShareResource>();
            await incomingShares.ForEachAsync(m => yourSharesSirOrMadam.Add(m));
            return yourSharesSirOrMadam;
        }
        public async Task<List<ShareResource>> GetPlayerCash(ModelReference playerRef)
        {
            var filter = Builders<ShareResource>.Filter.Eq(p => p.CurrentOwner.Id, playerRef.Id) &
            Builders<ShareResource>.Filter.Eq(p => p.ModelType, ModelTypes.Cash);
            var incomingCash = await _shares.FindAsync(filter);
            var yourCashSirOrMadam = new List<ShareResource>();
            await incomingCash.ForEachAsync(m => yourCashSirOrMadam.Add(m));
            return yourCashSirOrMadam;
        }
        public async Task<List<ModelReference>> CreateShares(ModelReference asset, int qty, ModelReference owner, ModelTypes type)
        {
            var sharesList = new List<ShareResource>();
            for (var i = 0; i < qty; i++)
            {
                sharesList.Add(new ShareResource()
                {
                    _assetId = asset.Id,
                    Name = $"Share of {asset.Name}",
                    ShareId = Guid.NewGuid().ToString(),
                    CurrentOwner = owner,
                    ModelType = type
                }
                );
            }
            // submit sharesList to the db
            await _shares.InsertManyAsync(sharesList).ConfigureAwait(true);
            return _mapper.Map<List<ShareResource>, List<ModelReference>>(sharesList);
        }
        public async Task<ModelReference> UpdateShares(List<ShareResource> shares)
        {
            try
            {
                var message = "";
                foreach (var share in shares)
                {
                    var filter = Builders<ShareResource>.Filter.Eq(s => s.ShareId, share.ShareId);
                    var update = Builders<ShareResource>.Update.Set(s => s.CurrentOwner.Id, share.CurrentOwner.Id);
                    await _shares.UpdateOneAsync(filter, update);
                    message += $"{share.ShareId}\n";
                }

                return new ModelReference { Name = shares?[0]?.Name, ModelType = ModelTypes.Share, Message = message += "shares were successfully updated" };
            }
            catch (Exception e)
            {
                return new ModelReference { Message = $"Something went wrong while updating the shares: {e.Message}" };
            }
        }
        public ModelReference ToRef(Share share) =>
            _mapper.Map<Share, ModelReference>(share);
        public List<ModelReference> ToRef(List<Share> shares) =>
            _mapper.Map<List<Share>, List<ModelReference>>(shares);
        public ModelReference ResToRef(ShareResource share) =>
            _mapper.Map<ShareResource, ModelReference>(share);
        public List<ModelReference> ResToRef(List<ShareResource> shares) =>
            _mapper.Map<List<ShareResource>, List<ModelReference>>(shares);
    }
    public interface IShareService
    {
        Task<IAsyncCursor<ShareResource>> GetAsync();
        Task<List<ShareResource>> GetAssetSharesAsync(AssetResource asset);
        Task<IAsyncCursor<ShareResource>> GetAsync(Guid id);
        Task<List<ShareResource>> GetAsync(List<Guid> shares);
        Task<IAsyncCursor<ShareResource>> GetNonCashAsync();
        Task<List<ShareResource>> GetPlayerShares(ModelReference playerRef);
        Task<List<ShareResource>> GetPlayerCash(ModelReference playerRef);
        Task<List<ModelReference>> CreateShares(ModelReference asset, int qty, ModelReference owner, ModelTypes type);
        Task<ModelReference> UpdateShares(List<ShareResource> shares);
        ModelReference ToRef(Share share);
        List<ModelReference> ToRef(List<Share> shares);
        ModelReference ResToRef(ShareResource share);
        List<ModelReference> ResToRef(List<ShareResource> shares);
    }
}
