using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Controllers;

[Route("/api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<string> GetById([FromRoute] string id)
    {
        if (string.IsNullOrEmpty(id) || id.Length > 10 || !id.All(char.IsLetterOrDigit))
        {
            return BadRequest("Parameter id is not well formed");
        }

        return Ok("product");
    }
}