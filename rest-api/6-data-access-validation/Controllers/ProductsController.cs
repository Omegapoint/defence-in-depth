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

        var canRead = User.HasClaim(c => c.Type == "urn:permissions:products:read" && c.Value == "true");

        if (!canRead)
        {
            return Forbid();
        }

        // Get product by id, here we have just hard coded a product for the Swedish market.
        var product = new { Name = "product", Market = "se" };            

        if (User.HasClaim(claim => 
                claim.Type == "urn:permissions:market" && 
                claim.Value == product.Market))
        {
            return Ok(product);
        }

        return NotFound();
    }
}