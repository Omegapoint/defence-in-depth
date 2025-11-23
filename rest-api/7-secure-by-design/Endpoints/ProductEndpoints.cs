using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;

namespace Defence.In.Depth.Endpoints;

public static class ProductEndpoints
{
    public static void RegisterProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{id}", GetById);
    }

    private static async Task<IResult> GetById(string id, IProductService productService)
    {
        if (!ProductId.IsValidId(id))
        {
            return Results.BadRequest("Id is not valid.");
        }

        var productId = new ProductId(id);
            
        var (product, result) = await productService.GetById(productId);

        switch (result)
        {
            case ReadDataResult.NoAccessToOperation:
                return Results.Forbid();
                
            case ReadDataResult.NotFound:
            case ReadDataResult.NoAccessToData:
                return Results.NotFound();

            case ReadDataResult.Success:
                if (product == null) throw new InvalidOperationException("Product value expected for success result.");

                var contract = Mapper.Map(product);
            
                return Results.Ok(contract);
                
            default:
                throw new InvalidOperationException($"Result kind {result} is not supported");
        }
    }
}