using RiskGame.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RiskGame.API.Models.TransactionFolder;

namespace RiskGame.API.Persistence.Repositories
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly ITransactionContext _context;
        private readonly IDatabaseSettings _settings;
        private readonly string _sqlConnection;
        private readonly MySqlConnection _transactions;
        public TransactionRepo(ITransactionContext context)
        {
            _context = context;
        }
        public void AddTransaction(TradeTicket trade)
        {
            var newTrade = new TransactionResource
            {
                TradeId = trade.TradeId,
                Buyer = trade.Buyer.Id,
                Seller = trade.Seller.Id,
                Asset = trade.Asset.Id,
                Price = trade.Cash / trade.Shares,
                Shares = trade.Shares,
                TradeTime = trade.TradeTime
            };
            _context.AddTrade(newTrade);
        }
        public TransactionResource GetTrade(Guid transactionId, string[] columns) =>_context.GetTrade(transactionId, columns);
        public List<TransactionResource> GetTradesSince(Guid gameId, Guid assetId, DateTime? since) =>_context.GetMany(gameId, assetId, since);
        public void CleanSlate() =>_context.CleanSlate();
    }
    public interface ITransactionRepo
    {
        void AddTransaction(TradeTicket trade);
        TransactionResource GetTrade(Guid transactionId, string[] columns);
        List<TransactionResource> GetTradesSince(Guid gameId, Guid assetId, DateTime? since);
        void CleanSlate();
    }
}
