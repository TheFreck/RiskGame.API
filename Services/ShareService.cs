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

namespace RiskGame.API.Services
{
    public class ShareService : IShareService
    {
        private readonly IShareRepo _shareRepo;
        private readonly IMapper _mapper;
        public ShareService(IShareRepo shareRepo, IMapper mapper)
        {
            _mapper = mapper;
            _shareRepo = shareRepo;
        }
        public IQueryable<ShareResource> GetQueryableShares(Guid assetId) => _shareRepo.GetMany().Where(s => s._assetId == assetId);
        public ShareResource GetSpecificShare(Guid id) => _shareRepo.GetOne(id);
        public IQueryable<ShareResource> GetSpecificShares(List<Guid> shares) => _shareRepo.GetManySpecific(shares);
        public int GetPlayerShareCount(Guid playerId, Guid assetId) => _shareRepo.GetMany().Where(s => s.CurrentOwner.Id == playerId).Where(s => s._assetId == assetId).Count();
        public List<ShareResource> GetPlayerShares(ModelReference playerRef, AssetResource asset, int qty) => _shareRepo.GetMany().Where(s => s.CurrentOwner == playerRef).Where(s => s._assetId == asset.AssetId).Take(qty).ToList();
        public List<ShareResource> GetAllPlayerShares(ModelReference playerRef, AssetResource asset) => _shareRepo.GetMany().Where(s => s.CurrentOwner.Id == playerRef.Id).Where(s => s._assetId == asset.AssetId).ToList();
        public List<Guid> CreateShares(ModelReference  asset, int qty, ModelReference owner, ModelTypes type)
        {
            var listOut = new List<Guid>();
            for (var i = 0; i < qty; i++)
            {
                var shareId = Guid.NewGuid();
                listOut.Add(shareId);
                _shareRepo.CreateOne(new ShareResource()
                {
                    _assetId = asset.Id,
                    Name = $"Share of {asset.Name}",
                    ShareId = shareId,
                    CurrentOwner = owner,
                    ModelType = type
                });
            }
            return listOut;
        }
        public UpdateResult UpdateShares(List<Guid> shares, UpdateDefinition<ShareResource> update) => _shareRepo.UpdateMany(shares, update).Result;
        public DeleteResult ShredShares(Guid assetId) => _shareRepo.DeleteAssetShares(assetId).Result;
        public ModelReference  ToRef(Share share) => _mapper.Map<Share, ModelReference >(share);
        public List<ModelReference > ToRef(List<Share> shares) => _mapper.Map<List<Share>, List<ModelReference >>(shares);
        public ModelReference  ResToRef(ShareResource share) => _mapper.Map<ShareResource, ModelReference >(share);
        public List<ModelReference > ResToRef(List<ShareResource> shares) => _mapper.Map<List<ShareResource>, List<ModelReference >>(shares);
    }
    public interface IShareService
    {
        IQueryable<ShareResource> GetQueryableShares(Guid assetId);
        ShareResource GetSpecificShare(Guid id);
        IQueryable<ShareResource> GetSpecificShares(List<Guid> shares);
        int GetPlayerShareCount(Guid playerId, Guid assetId);
        List<ShareResource> GetPlayerShares(ModelReference  playerRef, AssetResource asset, int qty);
        List<ShareResource> GetAllPlayerShares(ModelReference  playerRef, AssetResource asset);
        List<Guid> CreateShares(ModelReference asset, int qty, ModelReference owner, ModelTypes type);
        UpdateResult UpdateShares(List<Guid> shares, UpdateDefinition<ShareResource> update);
        DeleteResult ShredShares(Guid assetId);
        ModelReference  ToRef(Share share);
        List<ModelReference > ToRef(List<Share> shares);
        ModelReference  ResToRef(ShareResource share);
        List<ModelReference > ResToRef(List<ShareResource> shares);
    }
}
