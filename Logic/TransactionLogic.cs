using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RiskGame.API.Logic
{
    public class TransactionLogic : ITransactionLogic
    {
        private readonly IShareService _shareService;
        private readonly IPlayerService _playerService;
        private readonly IAssetService _assetService;
        private readonly IMapper _mapper;
        public TransactionLogic(IShareService shareService, IPlayerService playerService, IAssetService assetService, IMapper mapper)
        {
            _shareService = shareService;
            _playerService = playerService;
            _assetService = assetService;
            _mapper = mapper;
        }

        //public async Task<List<ShareResource>> GetCash(List<ModelReference> cashRefs, int qty, Guid gameId)
        //{
        //    var hausRef = _playerService.GetHAUSRef(gameId);
        //    var cashGuids = new List<Guid>();
        //    var hausCash = _shareService.GetAllPlayerShares(hausRef, ModelTypes.Cash).Result;
        //    var haus = _playerService.GetHAUS(gameId);
        //    if (cashRefs?.Count > 0)
        //    {
        //        //Console.WriteLine("cashRefs");
        //        foreach (var id in cashRefs)
        //        {
        //            cashGuids.Add(id.Id);
        //        }
        //    }
        //    else
        //    {
        //        for(var i = 0; i<qty; i++)
        //        {
        //            cashGuids.Add(Guid.Parse(hausCash[i].ShareId));
        //        }
        //    }
        //    var incomingCash = await _shareService.GetAsync(cashGuids);
        //    var tradeCash = new List<ShareResource>();
        //    incomingCash.ForEach(c => tradeCash.Add(c));
        //    return tradeCash;
        //}
        //public async Task<List<ShareResource>> GetShares(List<ModelReference> shareRefs, int qty, Guid gameId)
        //{
        //    var shareGuids = new List<Guid>();
        //    var hausRef = _playerService.GetHAUSRef(gameId);
        //    var hausPort = _shareService.GetPlayerShares(hausRef,ModelTypes.Share).Result;
        //    if (shareRefs?.Count > 0)
        //    {
        //        foreach(var id in shareRefs)
        //        {
        //            shareGuids.Add(id.Id);
        //        }
        //    }
        //    else
        //    if(shareRefs == null && hausPort.Count > qty)
        //    {
        //        for(var i = 0; i < qty; i++)
        //        {
        //            shareGuids.Add(Guid.Parse(hausPort[i].ShareId));
        //        }
        //    }
        //    else
        //    {
        //        return new List<ShareResource>();
        //    }
        //    var incomingShares = await _shareService.GetAsync(shareGuids);
        //    var tradeShares = new List<ShareResource>();
        //    incomingShares.ForEach(s => tradeShares.Add(s));
        //    return tradeShares;
        //}
        public List<ShareResource> TransferShares(PlayerResource receiver, List<ShareResource> shares, int price)
        {
            if(shares[0].ModelType == ModelTypes.Share)
            {
                foreach(var share in shares)
                {
                    share.History.Add(new TradeRecord
                    {
                        Buyer = _playerService.ResToRef(receiver),
                        Asset = _shareService.ResToRef(share),
                        ShareId = Guid.Parse(share.ShareId),
                        Price = price,
                        TradeTime = DateTime.Now
                    });
                    share.CurrentOwner = _playerService.ResToRef(receiver);
                }

            }
            else if(shares[0].ModelType == ModelTypes.Cash)
            {
                foreach(var share in shares)
                {
                    share.CurrentOwner = _playerService.ResToRef(receiver);
                }
            }
            return shares;
        }
    }
    public interface ITransactionLogic
    {
        //Task<List<ShareResource>> GetCash(List<ModelReference> cashRefs, int qty, Guid gameId);
        //Task<List<ShareResource>> GetShares(List<ModelReference> shareRefs, int qty, Guid gameId);
        List<ShareResource> TransferShares(PlayerResource receiver, List<ShareResource> shares, int price);
    }
}
