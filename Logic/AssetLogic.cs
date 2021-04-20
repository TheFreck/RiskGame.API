using AutoMapper;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using RiskGame.API.Models.SharesFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;
using RiskGame.API.Persistence;
using RiskGame.API.Models.PlayerFolder;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace RiskGame.API.Logic
{
    public class AssetLogic : IAssetLogic
    {
        private readonly IAssetService _assetService;
        private readonly IPlayerService _playerService;
        private readonly IShareService _shareService;
        private readonly IMapper _mapper;
        private readonly Random randy;
        public AssetLogic(IAssetService assetService, IPlayerService playerService, IShareService shareService, IMapper mapper)
        {
            _assetService = assetService;
            _playerService = playerService;
            _shareService = shareService;
            _mapper = mapper;
            randy = new Random();
        }

        public void PayDividend(ModelReference assetRef)
        {
            // calculate dividend
            var incomeSheet = CalculateDividend(assetRef);
            var asset = _assetService.GetAsset(assetRef.Id, ModelTypes.Asset);
            var hausRef = _playerService.GetHAUSRef(asset.GameId);
            var shares = _shareService.GetQueryableShares(assetRef.Id).ToList();
            var cash = _assetService.GetGameCash(asset.GameId);
            var cashRef = _assetService.ResToRef(cash);
            var dividends = _shareService.CreateShares(cashRef, incomeSheet.Dividends, hausRef, ModelTypes.Cash).Result;
            var dividendsPerShare = dividends.Count / shares.Count;
            // pay dividends on each share
            var shareDividends = new List<Share>();
            foreach(var share in shares)
            {
                var owner = share.CurrentOwner;
                for (var i=0; i<dividendsPerShare; i++)
                {
                    dividends[0].CurrentOwner = owner;
                    shareDividends.Add(dividends[0]);
                    dividends.RemoveAt(0);
                }
            }
            foreach(var div in dividends)
            {
                div.CurrentOwner = hausRef;
                shareDividends.Add(div);
            }
            _shareService.UpdateShares(_mapper.Map<List<Share>, List<ShareResource>>(shareDividends));
            // add leftover dividend cash back into the company value
            asset.MostRecentValue += incomeSheet.EquityGrowth + dividends.Count;
            _assetService.Replace(Guid.Parse(asset.AssetId), _mapper.Map<AssetResource, Asset>(asset));
        }
        private IncomeSheet CalculateDividend(ModelReference assetRef)
        {
            var asset = _assetService.GetAsset(assetRef.Id, ModelTypes.Asset);
            var recentValue = asset.MostRecentValue;
            var currentValue = asset.CompanyAsset.Value * asset.Debt;
            var grossIncome = currentValue - recentValue;
            var debtService = .02 * asset.Debt * recentValue;
            var growthAfterDebtService = grossIncome - debtService;
            var dividends = (int)Math.Floor(growthAfterDebtService * .5);
            var equityGrowth = growthAfterDebtService - dividends;
            return new IncomeSheet
            {
                GrossIncome = grossIncome,
                DebtService = debtService,
                Dividends = dividends,
                EquityGrowth = equityGrowth
            };
        }
    }
    public interface IAssetLogic
    {
        void PayDividend(ModelReference assetRef);
    }
}
