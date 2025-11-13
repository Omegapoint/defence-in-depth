using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Controllers;

[Route("/api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService productService;


    public ProductsController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDataContract>> GetById([FromRoute] string id)
    {
        if (!ProductId.IsValidId(id))
        {
            return BadRequest("Id is not valid.");
        }

        var productId = new ProductId(id);
            
        var (product, result) = await productService.GetById(productId);

        switch (result)
        {
            case ReadDataResult.NoAccessToOperation:
                return Forbid();
                
            case ReadDataResult.NotFound:
            case ReadDataResult.NoAccessToData:
                return NotFound();

            case ReadDataResult.Success:
                ProductDataContract contract = new(product?.Id.ToString(), product?.Name.ToString());
            
                return Ok(contract);
                
            default:
                throw new InvalidOperationException($"Result kind {result} is not supported");
        }
    }
}