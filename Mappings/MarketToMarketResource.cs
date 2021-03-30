using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Entities;
using RiskGame.API.Models.MarketFolder;

namespace RiskGame.API.Mappings
{
    public class MarketToMarketResource : Profile
    {
        public MarketToMarketResource()
        {
            CreateMap<Market, MarketResource>();
        }
    }
    public class MarketResourceToMarket : Profile
    {
        public MarketResourceToMarket()
        {
            CreateMap<MarketResource, Market>();
        }
    }
    public class MarketMetricsToMarketResource : Profile
    {
        public MarketMetricsToMarketResource()
        {
            CreateMap<MarketMetrics, MarketResource>();
        }
    }
    public class MarketResourceToMarketMetrics : Profile
    {
        public MarketResourceToMarketMetrics()
        {
            CreateMap<MarketResource, MarketMetrics>();
        }
    }
    public class MarketToMarketMetrics : Profile
    {
        public MarketToMarketMetrics()
        {
            CreateMap<Market, MarketMetrics>();
        }
    }
    public class MarketMetricsToMarket : Profile
    {
        public MarketMetricsToMarket()
        {
            CreateMap<MarketMetrics, Market>();
        }
    }
}
