using AutoMapper;
using RiskGame.API.Engine;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Logic
{
    public class AssetLogic
    {
        private readonly IAssetService _assetService;
        private readonly IPlayerService _playerService;
        private readonly IShareService _shareService;
        private readonly IEconomy _economy;
        private readonly IMapper _mapper;
        private readonly Random randy;
        public AssetLogic(IAssetService assetService, IPlayerService playerService, IShareService shareService, IEconomy economy, IMapper mapper)
        {
            _assetService = assetService;
            _playerService = playerService;
            _shareService = shareService;
            _economy = economy;
            _mapper = mapper;
            randy = new Random();
        }

        public CompanyAsset CompanyTurn(CompanyAsset company)
        {
            var metrics = _economy.GetMetrics();
            company.Value *= IndustryGrowth(company.PrimaryIndustry, metrics) * IndustryGrowth(company.SecondaryIndustry, metrics);
            return company;
        }
        private double IndustryGrowth(IndustryTypes industry, EconMetrics metrics)
        {
            switch (industry)
            {
                case IndustryTypes.Red:
                    return metrics.Red;
                case IndustryTypes.Orange:
                    return metrics.Orange;
                case IndustryTypes.Yellow:
                    return metrics.Yellow;
                case IndustryTypes.Green:
                    return metrics.Green;
                case IndustryTypes.Blue:
                    return metrics.Blue;
                case IndustryTypes.Violet:
                    return metrics.Violet;
                default:
                    return (metrics.Red + metrics.Orange + metrics.Yellow + metrics.Green + metrics.Blue + metrics.Violet) / 6;
            }
        }
    }
}
