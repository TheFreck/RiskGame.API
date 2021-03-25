using AutoMapper;
using RiskGame.API.Entities;
using RiskGame.API.Entities.Enums;
using RiskGame.API.Models.AssetFolder;
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
            switch (company.Industry)
            {
                case IndustryTypes.Red:
                    company.Value = metrics.Red * company.Cyclicality;
                    break;
                case IndustryTypes.Orange:
                    company.Value = metrics.Orange * company.Cyclicality;
                    break;
                case IndustryTypes.Yellow:
                    company.Value = metrics.Yellow * company.Cyclicality;
                    break;
                case IndustryTypes.Green:
                    company.Value = metrics.Green * company.Cyclicality;
                    break;
                case IndustryTypes.Blue:
                    company.Value = metrics.Blue * company.Cyclicality;
                    break;
                case IndustryTypes.Violet:
                    company.Value = metrics.Violet * company.Cyclicality;
                    break;
                default:
                    break;
            }
            return company;
        }
    }
}
