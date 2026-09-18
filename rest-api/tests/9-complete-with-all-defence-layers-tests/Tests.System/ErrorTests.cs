using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace CompleteWithAllDefenceLayers.Tests.System;

[Trait("Category", "System")]
public class ErrorTests(ITestOutputHelper output) : BaseTests(output)
{
    [Fact]
    public async Task ThrowWithValidToken_ShouldReturn500AndNoDetails()
    {
        var httpClient = CreateAuthenticatedHttpClient();
        
        var response = await httpClient.PutAsync("/api/error", null);
        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains("https://tools.ietf.org/html/rfc9110#section-15.6.1", responseContent);
    }
}