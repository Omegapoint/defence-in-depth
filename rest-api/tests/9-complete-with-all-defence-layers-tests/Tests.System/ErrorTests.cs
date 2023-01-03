using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace CompleteWithAllDefenceLayers.Tests.System;

[Trait("Category", "System")]
public class ErrorTests : BaseTests
{
    public ErrorTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task ThrowWithValidToken_ShouldReturn500AndNoDetails()
    {
        var httpClient = CreateAuthenticatedHttpClient();
        
        var response = await httpClient.PutAsync("/api/error", null);
        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        // Note that this will fail when running localhost (in development), 
        // since ASP.NET Core will return exception details when ASPNETCORE_ENVIRONMENT=Development
        Assert.True(string.IsNullOrEmpty(responseContent));
    }
}