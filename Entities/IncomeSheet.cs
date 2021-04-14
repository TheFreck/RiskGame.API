using RiskGame.API.Models.SharesFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Entities
{
    public class IncomeSheet
    {
        public double GrossIncome { get; set; }
        public double DebtService { get; set; }
        public int Dividends { get; set; }
        public double EquityGrowth { get; set; }
    }
}
