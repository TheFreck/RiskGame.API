using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskGame.API.Controllers
{
    [Route("api/codewar")]
    [ApiController]
    [Produces("application/json")]
    public class CodeWarController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("Yup. You made it here");
        }
        [HttpPost("to")]
        public ActionResult<string> ToPost([FromBody]int arabic)
        {
            return ToRoman(arabic);
        }
        [HttpPost("from")]
        public ActionResult<int> FromPost([FromBody] string roman)
        {
            return FromRoman(roman);
        }
        public static ActionResult<string> ToRoman(int n)
        {
            string roman, preM, preD, preC, preL, preX, preV;
            roman = preM = preD = preC = preL = preX = preV = String.Empty;
            var m = Math.Floor(n / 1000.0);
            var mLeft = n % 1000;
            if (mLeft > 899)
            {
                preM="CM";
                mLeft-=900;
            }
            for(var mi = 0; mi < m; mi++)
            {
                roman += "M";
            }
            roman += preM;
            var d = Math.Floor(mLeft / 500.0);
            var dLeft = mLeft % 500;
            if(dLeft > 399)
            {
                preD = "CD";
                dLeft -= 400;
            }
            for(var di=0; di < d; di++)
            {
                roman += "D";
            }
            roman += preD;
            var c = Math.Floor(dLeft / 100.0);
            var cLeft = dLeft % 100;
            if(cLeft > 89)
            {
                preC = "XC";
                cLeft -= 90;
            }
            for(var ci=0; ci < c; ci++)
            {
                roman += "C";
            }
            roman += preC;
            var l = Math.Floor(cLeft / 50.0);
            var lLeft = cLeft % 50;
            if(lLeft > 39)
            {
                preL = "XL";
                lLeft -= 40;
            }
            for(var li=0; li<l; li++)
            {
                roman += "L";
            }
            roman += preL;
            var x = Math.Floor(lLeft / 10.0);
            var xLeft = lLeft % 10;
            if(xLeft > 8)
            {
                preX = "IX";
                xLeft -= 9;
            }
            for(var xi=0; xi < x; xi++)
            {
                roman += "X";
            }
            roman += preX;
            var v = Math.Floor(xLeft / 5.0);
            var vLeft = xLeft % 5;
            if(vLeft > 3)
            {
                preV = "IV";
                vLeft -= 4;
            }
            for(var vi=0; vi<v; vi++)
            {
                roman += "V";
            }
            roman += preV;
            var i = vLeft;
            for(var ii=0; ii<i; ii++)
            {
                roman += "I";
            }
            return roman;
        }

        public static int FromRoman(string romanNumeral)
        {
            var arabic = 0;
            int M, D, C, L, X, V, I;
            M = D = C = L = X = V = I = 0;
            for (var i= 0; i < romanNumeral.Length; i++)
            {
                var current = romanNumeral[i].ToString().ToUpper();
                var previous = i!=0 ? romanNumeral[i - 1].ToString().ToUpper() : "";
                var next = i + 1 == romanNumeral.Length ? "" : romanNumeral[i + 1].ToString().ToUpper();
                switch (current)
                {
                    case "M":
                        if (previous == "C") arabic += 900;
                        else arabic += 1000;
                        break;
                    case "D":
                        if (previous == "C") arabic += 400;
                        else arabic += 500;
                        break;
                    case "C":
                        if (previous == "X") arabic += 90;
                        else if (next == "M" || next == "D") arabic += 0;
                        else arabic += 100;
                        break;
                    case "L":
                        if (previous == "X") arabic += 40;
                        else arabic += 50;
                        break;
                    case "X":
                        if (previous == "I") arabic += 9;
                        else if (next == "C" || next == "L") arabic += 0;
                        else arabic += 10;
                        break;
                    case "V":
                        if (previous == "I") arabic += 4;
                        else arabic += 5;
                        break;
                    case "I":
                        if (next == "I" || next == "") arabic += 1;
                        else if (next != "V" && next != "X") return -1;
                        break;
                    default:
                        return -1;
                }
            }
            return arabic;
        }
    }
}