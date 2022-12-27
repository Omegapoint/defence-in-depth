using System.Net;
using System.Security.Authentication;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.System;

// The Health controller tests verify that we run the correct version, has mandatory JWT access controll,
// handle exceptions and return recommended security headers.
[Trait("Category", "System")]
public class HealthTests
{
    private readonly Uri baseUri = new Uri("https://localhost:5001/");

    [Fact]
    public async Task LivenessAnonymous_ShouldReturn200AndCorrectVersion()
    {
        var client = new HttpClient();

        var response = await client.GetAsync(new Uri(baseUri, "/api/health/live"));
        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"version\":\"1.0.0\"", responseContent);
    }

    [Fact]
    public async Task ReadynessWithValidToken_ShouldReturn200()
    {
        var client = new TokenHttpClient();

        var response = await client.GetAsync(new Uri(baseUri, "/api/health/ready"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReadynessWithNoToken_ShouldReturn401()
    {
        var client = new HttpClient();

        var response = await client.GetAsync(new Uri(baseUri, "/api/health/ready"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ReadynessWithInvalidToken_ShouldReturn401()
    {
        //Use a token from jwt.io, which has an invalid issuer for our API
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c");
        
        var response = await client.GetAsync(new Uri(baseUri, "/api/health/ready"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ReadynessHttp_ShouldReturn405()
    {
        var client = new HttpClient();

        var response = await client.GetAsync(new Uri(new Uri("http://localhost/"), "/api/health/ready"));
        
        // Note that this will fail when running without NGINX 
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [Fact]
    public async Task LivenessAnonymous_ReturnsSecurityHeaders()
    {
        var client = new HttpClient();

        var response = await client.GetAsync(new Uri(baseUri, "/api/health/live"));

        // Verify according to https://cheatsheetseries.owasp.org/cheatsheets/REST_Security_Cheat_Sheet.html#security-headers
        // Note that this will fail when running without NGINX 
        Assert.Contains(response.Headers, h => h.Key == "Cache-Control" && h.Value.ToString() == "no-store");
        Assert.Contains(response.Headers, h => h.Key == "Content-Security-Policy" && h.Value.ToString() == "frame-ancestors 'none'");
        Assert.Contains(response.Headers, h => h.Key == "Content-Type" && h.Value.ToString() == "application/json");
        Assert.Contains(response.Headers, h => h.Key == "Strict-Transport-Security" && h.Value.ToString() == "max-age=31536000; includeSubDomains");
        Assert.Contains(response.Headers, h => h.Key == "X-Content-Type-Options" && h.Value.ToString() == "nosniff");
        Assert.Contains(response.Headers, h => h.Key == "X-Frame-Options" && h.Value.ToString() == "DENY");
    }

    [Fact]
    public async Task LivenessAnonymous_TLS_1_3_Only()
    {
        // Since we only allow TLS 1.3 we only need to verify that we only accept TLS 1.3 and 
        // there are no validation errors (accroding to .NET default policy) to assert sufficient TLS quality. 
        // For TLS 1.2 we should also verify strong chiphers according to e g 
        // https://openid.bitbucket.io/fapi/fapi-2_0-security-profile.html#section-5.2.2
        #pragma warning disable //Disable warnings for depricated TLS and SSL versions
        var handler13 = new HttpClientHandler();
        handler13.SslProtocols = SslProtocols.Tls13;
        var client13 = new HttpClient(handler13);
        var handler12 = new HttpClientHandler();
        handler12.SslProtocols = SslProtocols.Tls12;
        var client12 = new HttpClient(handler12);
        var handler11 = new HttpClientHandler();
        handler11.SslProtocols = SslProtocols.Tls11;
        var client11 = new HttpClient(handler11);
        var handlerSsl3 = new HttpClientHandler();
        handlerSsl3.SslProtocols = SslProtocols.Ssl3;
        var clientSsl3 = new HttpClient(handlerSsl3);
        var handlerSsl2 = new HttpClientHandler();
        handlerSsl2.SslProtocols = SslProtocols.Ssl2;
        var clientSsl2 = new HttpClient(handlerSsl2);
        #pragma warning restore

        var response13 = await client13.GetAsync(new Uri(baseUri, "/api/health/live"));

        // Note that this will fail when running without NGINX
        Assert.Equal(HttpStatusCode.OK, response13.StatusCode);
        await Assert.ThrowsAsync<Exception>(async () => await client12.GetAsync(new Uri(baseUri, "/api/health/live")));
        await Assert.ThrowsAsync<Exception>(async () => await client11.GetAsync(new Uri(baseUri, "/api/health/live")));
        await Assert.ThrowsAsync<Exception>(async () => await clientSsl3.GetAsync(new Uri(baseUri, "/api/health/live")));
        await Assert.ThrowsAsync<Exception>(async () => await clientSsl2.GetAsync(new Uri(baseUri, "/api/health/live")));
    }
}
