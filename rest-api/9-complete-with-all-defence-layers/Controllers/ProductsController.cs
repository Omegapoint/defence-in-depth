using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _9_complete_with_all_defence_layers
{
    [Route("/api/products")]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<string> GetById(string id)
        {
            return Ok("product");
        }
    }
}