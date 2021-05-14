using RiskGame.API.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Models.SharesFolder
{
    public class ShareInputs
    {
        public ModelReference Asset;
        public int Qty;
        public ModelReference Owner;
        public ModelTypes ModelType;
    }
}
