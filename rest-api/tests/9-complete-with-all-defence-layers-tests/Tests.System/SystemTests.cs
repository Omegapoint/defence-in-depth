using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.System;


[Trait("Category", "System")]
public class SystemTests
{
    private readonly Uri baseUri = new Uri("https://localhost:5001/");

    [Fact]
    public async Task GetProductById_ShouldReturn401_WhenNotAuthenticated()
    {
        var client = new HttpClient();
        var response = await client.GetAsync(new Uri(baseUri, "/api/products/se1"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_ShouldReturn200_WhenAuthenticated()
    {
        var client = new TokenHttpClient();
        var response = await client.GetAsync(new Uri(baseUri, "/api/products/se1"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
