using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models.TransactionFolder;

namespace RiskGame.API.Mappings
{
    public class TradeTicketToTradeClient : Profile
    {
        public TradeTicketToTradeClient()
        {
            CreateMap<TradeTicket, TradeClient>();
        }
    }
    public class TradeClientToTradeTicket : Profile
    {
        public TradeClientToTradeTicket()
        {
            CreateMap<TradeClient, TradeTicket>();
        }
    }
    public class TradeTicketToTransactionResource : Profile
    {
        public TradeTicketToTransactionResource()
        {
            CreateMap<TradeTicket, TransactionResource>().ForMember(member => member.Buyer, config => config.MapFrom(og => og.Buyer.Id))
                .ForMember(member => member.Seller, config => config.MapFrom(og => og.Seller.Id))
                .ForMember(member => member.Asset, config => config.MapFrom(og => og.Asset.Id))
                .ForMember(member => member.Price, config => config.MapFrom(og => og.Cash / og.Shares));

        }
    }
    public class TradeClientToTransactionResource : Profile
    {
        public TradeClientToTransactionResource()
        {
            CreateMap<TradeClient, TransactionResource>();
        }
    }
    public class TransactionResourceToTradeClient : Profile
    {
        public TransactionResourceToTradeClient()
        {
            CreateMap<TransactionResource, TradeClient>();
        }
    }
    public class TransactionResourceToTradeTicket : Profile
    {
        public TransactionResourceToTradeTicket()
        {
            CreateMap<TransactionResource, TradeTicket>();
        }
    }
}
