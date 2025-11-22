using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace CompleteWithAllDefenceLayers.Tests.System;

// The controller specific system test (int this case for the Products controller) verify 
// any authorization policies outside the domain e g using controller attributes or a gateway
[Trait("Category", "System")]
public class ProductsTests(ITestOutputHelper output) : BaseTests(output)
{
    [Fact]
    public async Task GetProductById_ShouldReturn401_WhenAnonymous()
    {
        var httpClient = CreateAnonymousHttpClient();

        var response = await httpClient.GetAsync("/api/products/se1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturn403_WhenWrongScope()
    {
        // Use a token with wrong scope, GetProductById requires products.read
        var httpClient = CreateAuthenticatedHttpClient(["products.write"]);

        var response = await httpClient.GetAsync("/api/products/se1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturn200_WhenCorrectScope()
    {
        var httpClient = CreateAuthenticatedHttpClient(["products.read"]);

        var response = await httpClient.GetAsync("/api/products/se1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
