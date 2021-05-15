using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.SharesFolder
{
    public class ShareInputs
    {
        public Guid GameId { get; set; }
        public ModelReference Asset { get; set; }
        public int Qty { get; set; }
        public ModelReference Owner { get; set; }
        public ModelTypes ModelType { get; set; }
    }
}
