using RiskGame.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RiskGame.API.Persistence.Repositories
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly IDatabaseSettings _settings;
        private readonly string _sqlConnection;
        private readonly MySqlConnection _transactions;
        public TransactionRepo(IDatabaseSettings settings, MySqlConnection sqlConnection)
        {
            _settings = settings;
            _sqlConnection = '@' + _settings.MySqlConnectionString;
            using var db = new MySqlConnection('@' + _settings.MySqlConnectionString);
            _transactions = db;
            _transactions.Open();
        }
        public void AddTransaction(TradeTicket trade)
        {
            using var cmd = new MySqlCommand();
            cmd.Connection = _transactions;
        }
    }
    public interface ITransactionRepo
    {

    }
}
