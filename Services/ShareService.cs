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
    public class ShareService
    {
        private readonly IMongoCollection<Share> _shares;
        private readonly IMapper _mapper;
        public ShareService(IDatabaseSettings settings, IMapper mapper)
        {
            _mapper = mapper;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _shares = database.GetCollection<Share>(settings.ShareCollectionName);
        }
        public async Task<IAsyncCursor<Share>> GetAsync() =>
            await _shares.FindAsync(share => true);
        public async Task<IAsyncCursor<Share>> GetAsync(Asset asset)
        {
            var filter = Builders<Share>.Filter.Eq("_assetId", asset.Id);
            return await _shares.FindAsync(filter);
        }
        public async Task<IAsyncCursor<Share>> GetAsync(Guid id)
        {
            var filter = Builders<Share>.Filter.Eq("Id", id.ToString());
            return await _shares.FindAsync(filter);
        }
        public async Task<List<Share>> GetNonCashAsync()
        {
            var incoming = await _shares.FindAsync(s => s.ModelType.Equals(ModelTypes.Share));
            var shares = new List<Share>();
            await incoming.ForEachAsync(s => shares.Add(s));
            Console.WriteLine("shares count: " + shares.Count);
            return shares;
        }
        public async Task<List<Share>> GetAsync(List<Guid> shares)
        {
            var filter = Builders<Share>.Filter.Where(s => shares.Contains(s.Id));
            var incoming = await _shares.FindAsync<Share>(filter);
            var returnShares = new List<Share>();
            await incoming.ForEachAsync(s => returnShares.Add(s));
            return returnShares;
        }
        public async Task<List<ModelReference>> CreateShares(ModelReference asset, int qty, ModelReference owner, ModelTypes type)
        {
            var sharesList = new List<Share>();
            for(var i=0; i<qty; i++)
            {
                sharesList.Add(new Share(
                    asset.Id,
                    $"Share of {asset.Name}",
                    owner,
                    type
                    ));
            }
            // submit sharesList to the db
            await _shares.InsertManyAsync(sharesList).ConfigureAwait(true);
            return ToRef(sharesList);
        }
        public async Task<ModelReference> UpdateShares(List<Share> shares)
        {
            try
            {
                var message = "";
                foreach(var share in shares)
                {
                    var filter = Builders<Share>.Filter.Eq(s => s.Id, share.Id);
                    var update = Builders<Share>.Update.Set(s => s.CurrentOwner, share.CurrentOwner);
                    await _shares.UpdateOneAsync(filter,update);
                    message += $"{share.Id}\n";
                }

                return new ModelReference { Name = shares?[0]?.Name, ModelType = ModelTypes.Share, Message = message += "shares were successfully updated" };
            }
            catch (Exception e)
            {
                return new ModelReference { Message = $"Something went wrong while updating the shares: {e.Message}" };
            }
        }
        public ModelReference ToRef(Share share) =>
            _mapper.Map<Share,ModelReference>(share);
        public List<ModelReference> ToRef(List<Share> shares) =>
            _mapper.Map<List<Share>, List<ModelReference>>(shares);
    }
}
