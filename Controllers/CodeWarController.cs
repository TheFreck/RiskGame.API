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
        public int Post([FromBody] int[][] matrix)
        {
            return Determinant(matrix);
        }
        public static int Determinant(int[][] matrix)
        {
            foreach(var line in matrix)
            {
                var index = 0;
                do
                {
                    var newIndex = GetIndex(index, line.Length);
                } while (true);
            }
            return 0;
        }
        public static int GetIndex(int oldIndex, int length)
        {
            return (oldIndex + 1) % length;
        }
    }
    public class Matrix
    {
        public int[][] matrix { get; set; }
    }
}
