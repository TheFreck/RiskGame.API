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
        public string[] Post([FromBody]Incoming arrays)
        {
            var yupWords = new List<string>();
            foreach (var word in arrays.array2)
            {
                Console.WriteLine("word: " + word);
                foreach (var sub in arrays.array1)
                {
                    Console.WriteLine("sub: " + sub);
                    if (word.Contains(sub)) yupWords.Add(sub);
                }
            }
            var arr = yupWords.ToHashSet().ToArray();
            Array.Sort(arr);
            return arr;
        }
    }
    public class Incoming
    {
        public string[] array1 { get; set; }
        public string[] array2 { get; set; }
    }
}
