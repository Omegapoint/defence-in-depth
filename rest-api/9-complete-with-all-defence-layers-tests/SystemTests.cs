using Xunit;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System;

namespace CompleteWithAllDefenceLayers.Test;


[Trait("Category", "System")]
public class SystemTests
{
    private readonly Uri baseUri = new Uri("http://localhost:5001/");

    [Fact]
    public async Task GetProductByIdShouldReturn401WhenNotAuthenticated()
    {
        var client = new HttpClient();
        var response = await client.GetAsync(new Uri(baseUri, "/api/products/productSE"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProductByIdShouldReturn200WhenAuthenticated()
    {
        var client = new TokenHttpClient();
        var response = await client.GetAsync(new Uri(baseUri, "/api/products/productSE"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
