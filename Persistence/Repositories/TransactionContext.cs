using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RiskGame.API.Models.TransactionFolder;

namespace RiskGame.API.Persistence.Repositories
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
                        one.TradeTime = Convert.ToDateTime(reader["trade_time"]);
                    }
                    conn.Close();
                }
            }
            return one;
        }
        //
        // get many
        public List<TransactionResource> GetMany(Guid gameId, Guid assetId, DateTime since)
        {
            var many = new List<TransactionResource>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM asset_trades where 'game_id' = {gameId} where 'asset_id' = {assetId} where 'trade_time' >= {since} order by 'sequence' desc", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        many.Add(new TransactionResource
                        {
                            Sequence = Convert.ToInt32(reader["sequence"]),
                            TradeId = Guid.Parse(reader["trade_id"].ToString()),
                            GameId = Guid.Parse(reader["game_id"].ToString()),
                            Buyer = Guid.Parse(reader["buyer"].ToString()),
                            Seller = Guid.Parse(reader["seller"].ToString()),
                            Asset = Guid.Parse(reader["asset"].ToString()),
                            Price = Convert.ToDecimal(reader["price"]),
                        });
                    }
                    conn.Close();
                }
            }
            return many;
        }
        //
        // get all
        public List<TransactionResource> GetAll(Guid gameId)
        {
            var transactions = new List<TransactionResource>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"select * from transactions.asset_trades where 'game_id' = {gameId};", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(new TransactionResource
                        {
                            TradeId = Guid.Parse(reader["trade_id"].ToString()),
                            GameId = Guid.Parse(reader["game_id"].ToString()),
                            Buyer = Guid.Parse(reader["buyer"].ToString()),
                            Seller = Guid.Parse(reader["seller"].ToString()),
                            Asset = Guid.Parse(reader["asset"].ToString()),
                            Price = Convert.ToInt32(reader["price"]),
                        });
                    }
                    conn.Close();
                }
            }
            return transactions;
        }
        //
        // add transaction
        public void AddTrade(TransactionResource trade)
        {
            using (MySqlConnection conn = GetConnection())
            {
                var commandText = $"INSERT INTO `transactions`.`asset_trades` ( `trade_id`, `game_id`, `buyer`, `seller`, `asset`, `price` ) values('{trade.TradeId}','{trade.GameId}','{trade.Buyer}','{trade.Seller}','{trade.Asset}','{trade.Price}')";
                MySqlCommand cmd = new MySqlCommand(commandText, conn);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                }
                conn.Close();
            }
        }
        //
        // delete transactions
        public void CleanSlate()
        {
            using (MySqlConnection conn = GetConnection())
            {
                var testText = "SELECT * FROM 'transactions'.'asset_trades';";
                conn.Open();
                MySqlCommand testCommand = new MySqlCommand(testText, conn);
                var testReader = testCommand.ExecuteReader();
                var commandText = String.Empty;
                testReader.Read();
                
                if (testReader.FieldCount > 0) commandText = "DROP TABLE 'transactions'.'asset_trades';";
                
                commandText += "CREATE TABLE 'transactions'.'asset_trades' (" +
                                    "  'sequence' INT(11) NOT NULL AUTO_INCREMENT," +
                                    "  'trade_time' TIMESTAMP DEFAULT CURRENT_TIMESTAMP," +
                                    "  'trade_id' VARCHAR(36) NOT NULL," +
                                    "  'game_id' VARCHAR(36) NOT NULL," +
                                    "  'buyer' VARCHAR(36) NOT NULL," +
                                    "  'seller' VARCHAR(36) NOT NULL," +
                                    "  'asset' VARCHAR(36) NOT NULL," +
                                    "  'price' DECIMAL(11,3) UNSIGNED NOT NULL," +
                                    "  UNIQUE INDEX 'sequence' ('sequence' ASC)," +
                                    "  PRIMARY KEY ('sequence')," +
                                    "  INDEX 'game_id' ('game_id' ASC)," +
                                    "  INDEX 'buyer' ('buyer')," +
                                    "  INDEX 'seller' ('seller')," +
                                    "  INDEX 'asset' ('asset'));";
                MySqlCommand cmd = new MySqlCommand(commandText, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                }
                conn.Close();
            }
        }
    }
}