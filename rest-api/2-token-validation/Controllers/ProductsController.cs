using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Controllers;

[Route("/api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<string> GetById([FromRoute] string id)
    {
        return Ok("product");
    }
}