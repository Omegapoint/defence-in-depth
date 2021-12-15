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

        var canRead = User.HasClaim(claim => 
            claim.Type == "urn:permission:product:read" && 
            claim.Value == "true");

        if (!canRead)
        {
            return Forbid();
        }

        var product = new { Name = "product", Market = "se" };            

        if (User.HasClaim(claim => 
                claim.Type == "urn:permission:market" && 
                claim.Value == product.Market))
        {
            return Ok(product);
        }

        return NotFound("product");
    }
}