namespace Defence.In.Depth.Endpoints;

public static class ProductEndpoints
{
    public static void RegisterProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{id}", GetById);
    }

        private static IResult GetById(string id, HttpContext httpContext)
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

        // Get product by id, here we have just hard coded a product for the Swedish market.
        var product = new { Name = "product", Market = "se" };            

        if (httpContext.User.HasClaim(claim => 
                claim.Type == "urn:permissions:market" && 
                claim.Value == product.Market))
        {
            return Results.Ok(product);
        }

        return Results.NotFound();
    }
}