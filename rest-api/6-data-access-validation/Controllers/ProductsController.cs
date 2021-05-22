using Defence.In.Depth.Domain.Model;
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
                return BadRequest();
            }

            var productId = new ProductId(id);
            var productName = new ProductName("my product");

            var product = new Product(productId, productName);

            if (!product.CanRead(User))
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
