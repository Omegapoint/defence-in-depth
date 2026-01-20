using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Endpoints;

public static class ProductEndpoints
{
    public static void RegisterProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{id}", GetById)
            .RequireAuthorization();
    }

    private static IResult GetById([FromRoute] string id)
    {
        if (string.IsNullOrEmpty(id) || id.Length > 10 || !id.All(char.IsLetterOrDigit))
        {
            return Results.BadRequest("Parameter id is not well formed");
        }

        return Results.Ok("product");
    }
}