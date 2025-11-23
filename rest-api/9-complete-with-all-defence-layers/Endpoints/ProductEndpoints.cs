using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;

namespace Defence.In.Depth.Endpoints;

public static class ProductEndpoints
{
    public static void RegisterProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{id}", GetById)
            .RequireAuthorization(ClaimSettings.ProductsRead);
    }

    public static async Task<IResult> GetById(string id, IProductService productService)
    {
        if (!ProductId.IsValidId(id))
        {
            return Results.BadRequest("Id is not valid.");
        }

        var productId = new ProductId(id);
            
        var result = await productService.GetById(productId);
        
        return result.MapToHttpResult(Mapper.Map);
    }
}