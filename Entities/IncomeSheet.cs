using RiskGame.API.Models.SharesFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Entities
{
    public class IncomeSheet
    {
        public decimal GrossIncome { get; set; }
        public decimal DebtService { get; set; }
        public int Dividends { get; set; }
        public decimal EquityGrowth { get; set; }
    }
}
