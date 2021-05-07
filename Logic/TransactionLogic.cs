using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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


namespace RiskGame.API.Logic
{
    public class TransactionLogic : ITransactionLogic
    {
        private readonly IMapper _mapper;
        public TransactionLogic(IMapper mapper)
        {
            _mapper = mapper;
        }
        public ShareResource[] TransferShares(PlayerResource receiver, ShareResource[] shares, int price)
        {
            if(shares[0].ModelType == ModelTypes.Share)
            {
                foreach(var share in shares)
                {
                    share.History.Add(new TradeRecord
                    {
                        Buyer = _mapper.Map<PlayerResource,ModelReference>(receiver),
                        Asset = new ModelReference(share.Name[9..],share._assetId,ModelTypes.Asset),
                        ShareId = share.ShareId,
                        Price = price,
                        TradeTime = DateTime.Now
                    });
                    share.CurrentOwner = _mapper.Map<PlayerResource, ModelReference>(receiver);
                }

            }
            return shares;
        }
    }
    public interface ITransactionLogic
    {
        ShareResource[] TransferShares(PlayerResource receiver, ShareResource[] shares, int price);
    }
}
