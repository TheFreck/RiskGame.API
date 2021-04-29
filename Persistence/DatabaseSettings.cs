using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Persistence
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string PlayerCollectionName { get; set; }
        public string AssetCollectionName { get; set; }
        public string NewAssetCollectionName { get; set; }
        public string ShareCollectionName { get; set; }
        public string NewShareCollectionName { get; set; }
        public string EconomyCollectionName { get; set; }
        public string MarketCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Destructo { get; set; }
    }

    public interface IDatabaseSettings
    {
        string PlayerCollectionName { get; set; }
        string AssetCollectionName { get; set; }
        string NewAssetCollectionName { get; set; }
        string ShareCollectionName { get; set; }
        string NewShareCollectionName { get; set; }
        string EconomyCollectionName { get; set; }
        string MarketCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string Destructo { get; set; }
    }
}
