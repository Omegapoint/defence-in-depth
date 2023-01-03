using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace CompleteWithAllDefenceLayers.Tests.System;

// The controller specific system test (int this case for the Products controller) verify 
// any authorization policies outside the domain e g using controller attributes or a gateway
[Trait("Category", "System")]
public class ProductsTests : BaseTests
{
    public ProductsTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task GetProductById_ShouldReturn401_WhenWrongScope()
    {
        // Use a token with wrong scope, GetProductById requires products.read
        var httpClient = CreateAuthenticatedHttpClient(new[] {"products.write"});

        var response = await httpClient.GetAsync("/api/products/se1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturn200_WhenCorrectScope()
    {
        var httpClient = CreateAuthenticatedHttpClient(new[] {"products.read"});

        var response = await httpClient.GetAsync("/api/products/se1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
