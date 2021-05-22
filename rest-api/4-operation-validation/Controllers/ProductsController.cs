using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.AddControllers
{
    [Route("/api/products")]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<string> GetById(string id)
        {
            var canRead = User.HasClaim(c => c.Type == "urn:local:product:read" && c.Value == "true");

            if (!canRead)
            {
                return Forbid();
            }

            return Ok("product");
        }
    }
}
