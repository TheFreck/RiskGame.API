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
        public string Post([FromBody]Incoming colors) => Rgb(Convert.ToInt32(colors.R), Convert.ToInt32(colors.G), Convert.ToInt32(colors.B));
        public static string Rgb(int r, int g, int b)
        {
            var x = r < 0 ? 0.ToString("X2") : r > 255 ? 255.ToString("X2") : r.ToString("X2");
            var y = g < 0 ? 0.ToString("X2") : g > 255 ? 255.ToString("X2") : g.ToString("X2");
            var z = b < 0 ? 0.ToString("X2") : b > 255 ? 255.ToString("X2") : b.ToString("X2");
            return $"{x}{y}{z}";
        }
    }
    public class Incoming
    {
        public decimal R { get; set; }
        public decimal G { get; set; }
        public decimal B { get; set; }
    }
}
