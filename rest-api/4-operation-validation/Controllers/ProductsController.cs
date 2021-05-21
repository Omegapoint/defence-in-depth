using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _3_token_transformation.AddControllers
{
    [Route("/api/products")]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<string> GetById(string id)
        {
            var permissions = new Permissions(User);

            if (!permissions.CanGetProducts)
            {
                return Forbid();
            }

            return Ok("product");


            // Eller vi tror hellre på att köra ännu enklare:  Vi BESTÄMMER detta nu:


            var canGetProducts = User.HasClaim(c => c.Type == "urn:local:product:read" && c.Value == "true");

            if (!canGetProducts)
            {
                return Forbid();
            }
        }
    }
}
