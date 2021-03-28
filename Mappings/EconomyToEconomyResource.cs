using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiskGame.API.Models.EconomyFolder;
using AutoMapper;
using RiskGame.API.Entities;

namespace RiskGame.API.Mappings
{
    public class EconomyToEconomyResource : Profile
    {
        public EconomyToEconomyResource()
        {
            CreateMap<Economy, EconomyResource>();
        }
    }
    public class EconomyResourceToEconomy : Profile
    {
        public EconomyResourceToEconomy()
        {
            CreateMap<EconomyResource, Economy>();
        }
    }
    public class EconMetricsToEconomyResource : Profile
    {
        public EconMetricsToEconomyResource()
        {
            CreateMap<EconMetrics, EconomyResource>();
        }
    }
    public class EconomyResourceToEconMetrics : Profile
    {
        public EconomyResourceToEconMetrics()
        {
            CreateMap<EconomyResource, EconMetrics>();
        }
    }
    public class EconomyToEconMetrics : Profile
    {
        public EconomyToEconMetrics()
        {
            CreateMap<Economy, EconMetrics>();
        }
    }
    public class EconMetricsToEconomy : Profile
    {
        public EconMetricsToEconomy()
        {
            CreateMap<EconMetrics, Economy>();
        }
    }
}
