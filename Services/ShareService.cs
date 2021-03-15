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
        public async Task<IAsyncCursor<Share>> GetAsync(List<Guid> shares)
        {
            var filter = Builders<Share>.Filter.Where(s => shares.Contains(s.Id));
            return await _shares.FindAsync(filter);
        }
        public async Task<List<Share>> CreateShares(ModelReference asset, int qty, ModelReference owner)
        {
            var sharesList = new List<Share>();
            for(var i=0; i<qty; i++)
            {
                sharesList.Add(new Share(
                    asset.Id,
                    $"Share of {asset.Name}",
                    owner
                    ));
            }
            // submit sharesList to the db
            await _shares.InsertManyAsync(sharesList).ConfigureAwait(true);
            return sharesList;
        }
        public ModelReference UpdateShares(List<Share> shares)
        {
            try
            {
                var message = "";
                var success = shares.Count();
                foreach(var share in shares)
                {
                    var filter = Builders<Share>.Filter.Eq("Id", share.Id.ToString());
                    var outcome = _shares.FindOneAndReplace<Share>(filter,share);
                    message += outcome.Id.ToString() + "\n";
                    success--;
                }
                if (success > 0) throw new Exception("not all of the bills were removed from the buyer's wallet");
                return new ModelReference { Name = shares?[0]?.Name, ModelType = ModelTypes.Share, Message = message += "shares were successfully updated" };
            }
            catch (Exception e)
            {
                return new ModelReference { Message = $"Something went wrong while updating the shares: {e.Message}" };
            }
        }
    }
}
