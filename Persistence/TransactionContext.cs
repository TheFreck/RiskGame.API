using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RiskGame.API.Models.TransactionFolder;

namespace RiskGame.API.Persistence
{
    public class TransactionContext
    {
        public string ConnectionString { get; set; }
        public TransactionContext(string connectionString)
        {
            ConnectionString = connectionString;
        }
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        //
        // get one
        public TransactionResource GetTrade(Guid transactionId, string[] columns)
        {
            var one = new TransactionResource();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                // implement specific columns to pass back
                MySqlCommand cmd = new MySqlCommand($"select * from asset_trades where trade_id = {transactionId}", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        one.Sequence = Convert.ToInt32(reader["sequence"]);
                        one.TradeId = Guid.Parse(reader["trade_id"].ToString());
                        one.GameId = Guid.Parse(reader["game_id"].ToString());
                        one.Buyer = Guid.Parse(reader["buyer"].ToString());
                        one.Seller = Guid.Parse(reader["seller"].ToString());
                        one.Asset = Guid.Parse(reader["asset"].ToString());
                        one.Price = Convert.ToDecimal(reader["price"]);
                        //one.TradeTime = reader["trade_time"];
                        one.CompanyAssetValue = Convert.ToDecimal(reader["company_asset_value"]);
                    }
                }
            }
            return one;
        }
        //
        // get many
        //
        // get all
        //
        // add transaction
        public void AddTrade(TransactionResource trade)
        {

        }
        //
        // delete transactions
    }
}
