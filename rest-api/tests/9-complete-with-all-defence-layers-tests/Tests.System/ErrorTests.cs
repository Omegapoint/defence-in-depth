using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace CompleteWithAllDefenceLayers.Tests.System;

[Trait("Category", "System")]
public class ErrorTests(ITestOutputHelper output) : BaseTests(output)
{
    // The only members a generic ProblemDetails response for an unhandled exception should contain.
    // Anything else (e.g. "detail", "exception", "errors") could leak internal implementation details.
    private static readonly string[] AllowedProblemDetailsMembers = ["type", "title", "status", "traceId"];

    [Fact]
    public async Task ThrowWithValidToken_ShouldReturn500AndNoDetails()
    {
        var httpClient = CreateAuthenticatedHttpClient();

        var response = await httpClient.PutAsync("/api/error", null);
        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var problemDetails = JsonDocument.Parse(responseContent);
        Assert.Equal("https://tools.ietf.org/html/rfc9110#section-15.6.1", problemDetails.RootElement.GetProperty("type").GetString());
        Assert.Equal(500, problemDetails.RootElement.GetProperty("status").GetInt32());
        Assert.All(problemDetails.RootElement.EnumerateObject(), member => Assert.Contains(member.Name, AllowedProblemDetailsMembers));

        AssertNoExceptionDetails(responseContent);
    }

    [Fact]
    public async Task ThrowWithValidTokenAndNonJsonAccept_ShouldReturn500AndEmptyBody()
    {
        var httpClient = CreateAuthenticatedHttpClient();
        var request = new HttpRequestMessage(HttpMethod.Put, "/api/error");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

        var response = await httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.True(string.IsNullOrEmpty(responseContent));
    }

    private static void AssertNoExceptionDetails(string responseContent)
    {
        // The endpoint throws NotImplementedException, its name, message or stack trace must never reach the client.
        Assert.DoesNotContain("Exception", responseContent, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("   at ", responseContent);
        Assert.DoesNotContain(".cs:line", responseContent);
    }
}
