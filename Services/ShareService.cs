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
        public List<Share> Get() =>
            _shares.Find(share => true).ToList();
        public List<Share> Get(Asset asset)
        {
            var filter = Builders<Share>.Filter.Eq("_assetId", asset.Id);
            return _shares.Find<Share>(filter).ToList();
        }
        public Share Get(Guid id)
        {
            var filter = Builders<Share>.Filter.Eq("Id", id.ToString());
            return _shares.Find(filter).FirstOrDefault();
        }
        public ModelReference CreateShares(ModelReference asset, int qty)
        {
            var sharesList = new List<Share>();
            for(var i=0; i<qty; i++)
            {
                sharesList.Add(new Share(
                    asset.Id,
                    Guid.NewGuid(),
                    $"Share of {asset.Name}"
                    ));
            }
            try
            {
                // submit sharesList to the db
                _shares.InsertManyAsync(sharesList).ConfigureAwait(true);
                asset.Message = "Shares successfully created";
                return asset;
            }
            catch (Exception e)
            {
                asset.Message = $"Something went wrong in the creation of the shares: {e.Message}";
                return asset;
            }
        }
        public ModelReference UpdateShares(List<Share> shares)
        {
            try
            {
                foreach(var share in shares)
                {
                    var filter = Builders<Share>.Filter.Eq("Id", share.Id.ToString());
                    _shares.FindOneAndReplace<Share>(filter,share);
                }
                return new ModelReference { Name = shares[0].Name, ModelType = ModelTypes.Share, Message = "shares were successfully created" };
            }
            catch (Exception e)
            {
                return new ModelReference { Message = $"Something went wrong while updating the shares: {e.Message}" };
            }
        }
    }
}
