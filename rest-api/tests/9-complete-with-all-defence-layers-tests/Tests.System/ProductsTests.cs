using System.Net;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.System;

// The controller specific system test (int this case for the Products controller) verify 
// any authorization policies outside the domain e g using controller attributes or a gateway
[Trait("Category", "System")]
public class ProductsTests
{
    private readonly Uri baseUri = new Uri("https://localhost:5001/");

    [Fact]
    public async Task GetProductById_ShouldReturn401_WhenWrongScope()
    {
        // Use a token with wrong scope, GetProductById requires products.read
        var client = new TokenHttpClient("products.write");

        var response = await client.GetAsync(new Uri(baseUri, "/api/products/se1"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturn200_WhenCorrectScope()
    {
        var client = new TokenHttpClient("products.read");

        var response = await client.GetAsync(new Uri(baseUri, "/api/products/se1"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
