using RiskGame.API.Models.EconomyFolder;
using RiskGame.API.Models.MarketFolder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Mappings
{
    public class EconComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return (new CaseInsensitiveComparer()).Compare(((MarketMetrics)x).SequenceNumber, ((MarketMetrics)y).SequenceNumber);
        }
    }
}
