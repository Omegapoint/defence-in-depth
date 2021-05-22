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
            if (!ProductId.IsValidId(id))
            {
                return BadRequest(); // https://stackoverflow.com/q/3290182/291299
            }

            var productId = new ProductId(id);

            var product = new Product(productId);

            if (!product.CanRead(User))
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
