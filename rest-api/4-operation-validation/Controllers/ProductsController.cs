using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Controllers
{
    [Route("/api/products")]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<string> GetById([FromRoute] string id)
        {
            var canRead = User.HasClaim(c => c.Type == "urn:permission:product:read" && c.Value == "true");

            if (!canRead)
            {
                return Forbid();
            }

            return Ok("product");
        }
    }
}
