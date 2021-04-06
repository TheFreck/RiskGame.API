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
        [HttpPost]
        public List<string> Post([FromBody] string observed)
        {
            return GetPINs(observed);
        }
        public static List<string> GetPINs(string observed)
        {
            var alts = new Dictionary<char, List<string>>
            {
                {'1', new List<string>{"2","4"} },
                {'2', new List<string>{"1","3","5"} },
                {'3', new List<string>{"2","6"} },
                {'4', new List<string>{"1","5","7"} },
                {'5', new List<string>{"2","4","6","8"} },
                {'6', new List<string>{"3","5","9"} },
                {'7', new List<string>{"4","8"} },
                {'8', new List<string>{"5","7","9","0"} },
                {'9', new List<string>{"6","8"} },
                {'0', new List<string>{"8"} }
            };
            var codeArray = new List<List<string>>();
            var output = new List<string>();
            foreach(var digit in observed)
            {
                output.Add(digit.ToString());
                foreach(var item in alts[digit])
                {
                    var theItem = item;
                    var itemsToAdd = new List<string>();
                    foreach(var el in output)
                    {
                        itemsToAdd.Add(theItem += el);
                    }
                }
                codeArray.Add(alts[digit]);
            }

            var returnList = observed.Split().ToList();
            return returnList;
        }
    }
}
