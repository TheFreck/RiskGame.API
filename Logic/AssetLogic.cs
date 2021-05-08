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

using RiskGame.API.Persistence;
using RiskGame.API.Models.PlayerFolder;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using AutoMapper;

namespace RiskGame.API.Logic
{
    public class AssetLogic : IAssetLogic
    {
        private readonly IMapper _mapper;
        private readonly Random randy;
        public AssetLogic(IMapper mapper)
        {
            _mapper = mapper;
            randy = new Random();
        }

        public Dictionary<Guid,int> PayDividend(AssetResource asset, Share[] shares)
        {
            // calculate dividend
            var incomeSheet = CalculateDividend(asset);
            //var hausRef = _mapper.Map<PlayerResource, ModelReference>(haus);
            var dividendsPerShare = incomeSheet.Dividends / shares.Length;
            // pay dividends on each share
            var output = new Dictionary<Guid, int>();
            foreach(var share in shares)
            {
                if (!output.ContainsKey(share.CurrentOwner.Id)) output.Add(share.CurrentOwner.Id, 0);
                output[share.CurrentOwner.Id] += incomeSheet.Dividends / shares.Length;
            }
            return output;
        }
        private IncomeSheet CalculateDividend(AssetResource asset)
        {
            var recentValue = asset.MostRecentValue;
            var currentValue = asset.CompanyAsset.Value;
            var grossIncome = (currentValue - recentValue) * asset.Debt;
            var debtService = (decimal).02 * asset.Debt * recentValue;
            var growthAfterDebtService = grossIncome - debtService;
            var dividends = (int)Math.Floor(growthAfterDebtService / 2) - ((int)Math.Floor(growthAfterDebtService / 2) % asset.SharesOutstanding);
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
        Dictionary<Guid, int> PayDividend(AssetResource asset, Share[] shares);
    }
}
