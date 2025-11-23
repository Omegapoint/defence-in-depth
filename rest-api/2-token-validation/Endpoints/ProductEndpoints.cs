using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Endpoints;

public static class ProductEndpoints
{
    public static void RegisterProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{id}", ([FromRoute] string id) => Results.Ok("product"));
    }
}