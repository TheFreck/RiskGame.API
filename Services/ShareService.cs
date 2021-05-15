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
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Persistence.Repositories;
using System.Diagnostics;

namespace RiskGame.API.Services
{
    public class ShareService : IShareService
    {
        private readonly IShareRepo _shareRepo;
        private readonly IPlayerRepo _playerRepo;
        private readonly IMapper _mapper;
        public ShareService(IShareRepo shareRepo, IPlayerRepo playerRepo, IMapper mapper)
        {
            _mapper = mapper;
            _shareRepo = shareRepo;
            _playerRepo = playerRepo;
        }
        public IQueryable<ShareResource> GetQueryableShares(Guid assetId) => _shareRepo.GetMany().Where(s => s._assetId == assetId);
        public IQueryable<ShareResource> GetQueryableGameShares(Guid gameId) => _shareRepo.GetMany().Where(s => s.GameId == gameId);
        public ShareResource GetSpecificShare(Guid id) => _shareRepo.GetOne(id);
        public IQueryable<ShareResource> GetSpecificShares(List<Guid> shares) => _shareRepo.GetManySpecific(shares);
        public int GetPlayerShareCount(Guid playerId, Guid assetId) => _shareRepo.GetMany().Where(s => s.CurrentOwner.Id == playerId).Where(s => s._assetId == assetId).Count();
        public List<ShareResource> GetPlayerShares(ModelReference playerRef, AssetResource asset, int qty) => _shareRepo.GetMany().Where(s => s.CurrentOwner == playerRef).Where(s => s._assetId == asset.AssetId).Take(qty).ToList();
        public List<ShareResource> GetAllPlayerShares(ModelReference playerRef, AssetResource asset) => _shareRepo.GetMany().Where(s => s.CurrentOwner.Id == playerRef.Id).Where(s => s._assetId == asset.AssetId).ToList();
        public List<Guid> CreateShares(ModelReference  asset, int qty, ModelReference owner, ModelTypes type, Guid gameId)
        {
            var inputs = new ShareInputs
            {
                GameId = gameId,
                Asset = asset,
                Qty = qty,
                Owner = owner,
                ModelType = type,
            };
            var listOut = new SharesCreator(inputs, _shareRepo).CreateShares();

            return listOut;
        }
        public UpdateResult UpdateShares(List<Guid> shares, UpdateDefinition<ShareResource> update) => _shareRepo.UpdateMany(shares, update).Result;
        public DeleteResult ShredShares(Guid assetId) => _shareRepo.DeleteAssetShares(assetId).Result;
        public ModelReference  ToRef(Share share) => _mapper.Map<Share, ModelReference >(share);
        public List<ModelReference > ToRef(List<Share> shares) => _mapper.Map<List<Share>, List<ModelReference >>(shares);
        public ModelReference  ResToRef(ShareResource share) => _mapper.Map<ShareResource, ModelReference >(share);
        public List<ModelReference > ResToRef(List<ShareResource> shares) => _mapper.Map<List<ShareResource>, List<ModelReference >>(shares);
        //public List<ShareResource> CashExchange(List<ShareResource> sharesIn)
        //{
        //    // get denominations
        //    var onesComingIn = sharesIn.Where(s => s.Denomination == 0);
        //    var tensComingIn = sharesIn.Where(s => s.Denomination == 1);
        //    var hundredsComingIn = sharesIn.Where(s => s.Denomination == 2);
        //    var thousandsComingIn = sharesIn.Where(s => s.Denomination == 3);
        //    var tenThousandsComingIn = sharesIn.Where(s => s.Denomination == 4);
        //    var totalCashComingIn = tenThousandsComingIn.Count() * 10000 + thousandsComingIn.Count() * 1000 + hundredsComingIn.Count() * 100 + tensComingIn.Count() * 10 + onesComingIn.Count();

        //    var owner = sharesIn[0].CurrentOwner;
        //    var haus = _mapper.Map<PlayerResource,ModelReference>(_playerRepo.GetHAUS());
        //    var updateBase = Builders<ShareResource>.Update;
        //    var toHaus = updateBase.Set("CurrentOwner", haus);
        //    var update1 = updateBase.Set("Denomination", 0).Set("Name", "One").Set("CurrentOwner", owner);
        //    var update10 = updateBase.Set("Denomination", 1).Set("Name","Ten").Set("CurrentOwner",owner);
        //    var update100 = updateBase.Set("Denomination", 2).Set("Name","Hundred").Set("CurrentOwner", owner);
        //    var update1000 = updateBase.Set("Denomination", 3).Set("Name","Thousand").Set("CurrentOwner", owner);
        //    var update10000 = updateBase.Set("Denomination", 4).Set("Name","TenThousand").Set("CurrentOwner", owner);
        //    // build change
        //    var output = new List<ShareResource>();
            
        //    var ones = sharesIn.Count % 10;
        //    var singleShares = sharesIn.Take(ones).ToList();
        //    sharesIn.RemoveAll(s => singleShares.Contains(s));
        //    _shareRepo.UpdateMany(singleShares.Select(s => s.Id),update1);
        //    singleShares.ForEach(s => { s.Name = "One"; s.Denomination = 0; });
        //    output.AddRange(singleShares);

        //    var tens = (sharesIn.Count/10) % 10;
        //    var tenShares = sharesIn.Take(tens).ToList();
        //    sharesIn.RemoveAll(s => tenShares.Contains(s));
        //    _shareRepo.UpdateMany(tenShares.Select(s => s.Id), update10);
        //    tenShares.ForEach(s => { s.Name = "Ten"; s.Denomination = 1; });
        //    output.AddRange(tenShares);

        //    var hundreds = (sharesIn.Count / 100) % 10;
        //    var hundredShares = sharesIn.Take(hundreds).ToList();
        //    sharesIn.RemoveAll(s => hundredShares.Contains(s));
        //    _shareRepo.UpdateMany(hundredShares.Select(s => s.Id), update100);
        //    hundredShares.ForEach(s => { s.Name = "Hundred"; s.Denomination = 2; });
        //    output.AddRange(hundredShares);

        //    var thousands = (sharesIn.Count / 1000) % 10;
        //    var thousandShares = sharesIn.Take(thousands).ToList();
        //    sharesIn.RemoveAll(s => thousandShares.Contains(s));
        //    _shareRepo.UpdateMany(thousandShares.Select(s => s.Id), update1000);
        //    thousandShares.ForEach(s => { s.Name = "Thousand"; s.Denomination = 3; });
        //    output.AddRange(thousandShares);

        //    var tenThousands = (sharesIn.Count - thousands) / 10000;
        //    var tenThousandShares = sharesIn.Take(tenThousands).ToList();
        //    sharesIn.RemoveAll(s => tenThousandShares.Contains(s));
        //    _shareRepo.UpdateMany(tenThousandShares.Select(s => s.Id), update10000);
        //    tenThousandShares.ForEach(s => { s.Name = "TenThousand"; s.Denomination = 4; });
        //    output.AddRange(tenThousandShares);

        //    // all other cash shares go back to Haus
        //    _shareRepo.UpdateMany(sharesIn.Select(s => s.Id), toHaus);
        //    return output.ToList();
        //}
    }
    public interface IShareService
    {
        IQueryable<ShareResource> GetQueryableShares(Guid assetId);
        IQueryable<ShareResource> GetQueryableGameShares(Guid gameId);
        ShareResource GetSpecificShare(Guid id);
        IQueryable<ShareResource> GetSpecificShares(List<Guid> shares);
        int GetPlayerShareCount(Guid playerId, Guid assetId);
        List<ShareResource> GetPlayerShares(ModelReference  playerRef, AssetResource asset, int qty);
        List<ShareResource> GetAllPlayerShares(ModelReference  playerRef, AssetResource asset);
        List<Guid> CreateShares(ModelReference asset, int qty, ModelReference owner, ModelTypes type, Guid gameId);
        UpdateResult UpdateShares(List<Guid> shares, UpdateDefinition<ShareResource> update);
        DeleteResult ShredShares(Guid assetId);
        ModelReference  ToRef(Share share);
        List<ModelReference > ToRef(List<Share> shares);
        ModelReference  ResToRef(ShareResource share);
        List<ModelReference > ResToRef(List<ShareResource> shares);
        //List<ShareResource> CashExchange(List<ShareResource> sharesIn);
    }
}
