using AutoMapper;
using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Controllers;

[Route("/api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService productService;
    private readonly IMapper mapper;

    public ProductsController(IProductService productService, IMapper mapper)
    {
        this.productService = productService;
        this.mapper = mapper;
    }

    [Authorize(ClaimSettings.ProductsRead)]
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
                var contract = mapper.Map<ProductDataContract>(product);
            
                return Ok(contract);
                
            default:
                throw new InvalidOperationException($"Result kind {result} is not supported");
        }
    }
}