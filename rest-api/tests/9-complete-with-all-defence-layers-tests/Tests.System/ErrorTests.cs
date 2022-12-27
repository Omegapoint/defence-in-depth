using System.Net;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.System;

[Trait("Category", "System")]
public class ErrorTests
{
    private readonly Uri baseUri = new Uri("https://localhost:5001/");

    [Fact]
    public async Task ThrowWithValidToken_ShouldReturn500AndNoDetails()
    {
        var client = new TokenHttpClient();

        var response = await client.PutAsync(new Uri(baseUri, "/api/error"));
        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        // Note that this will fail when running localhost (in developlment), 
        // since ASP.NET Core will return exception details when ASPNETCORE_ENVIRONMENT=Development
        Assert.True(string.IsNullOrEmpty(responseContent));
    }
}