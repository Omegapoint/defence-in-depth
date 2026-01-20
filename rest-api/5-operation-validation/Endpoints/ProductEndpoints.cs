using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Endpoints;

public static class ProductEndpoints
{
    public static void RegisterProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{id}", GetById)
            .RequireAuthorization();
    }

    private static IResult GetById([FromRoute] string id, HttpContext httpContext)
    {
        if (string.IsNullOrEmpty(id) || id.Length > 10 || !id.All(char.IsLetterOrDigit))
        {
            return Results.BadRequest("Parameter id is not well formed");
        }

        var canRead = httpContext.User.HasClaim(c => c.Type == "urn:permissions:products:read" && c.Value == "true");

        if (!canRead)
        {
            return Results.Forbid();
        }

        return Results.Ok("product");
    }
}