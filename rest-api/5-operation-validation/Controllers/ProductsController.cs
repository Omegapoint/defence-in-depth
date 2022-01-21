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

        var canRead = User.HasClaim(c => c.Type == "urn:permissions:products:read" && c.Value == "true");

        if (!canRead)
        {
            return Forbid();
        }

        return Ok("product");
    }
}