using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CompleteWithAllDefenceLayers.Tests.System;

public class ClientCredentialsDelegatingHandler : DelegatingHandler
{
    private readonly Uri tokenUri;
    private readonly string clientId;
    private readonly string clientSecret;
    private readonly string[] scopes;

    private string? accessToken;

    public ClientCredentialsDelegatingHandler(IConfiguration configuration)
    : this(configuration, ["products.read"])
    {
    }
    
    public ClientCredentialsDelegatingHandler(IConfiguration configuration, string[] scopes)
    {
        tokenUri = new Uri(configuration["TokenUri"] ?? throw new InvalidOperationException("Null token URI."));
        clientId = configuration["ValidClient:ClientId"]!;
        clientSecret = configuration["ValidClient:ClientSecret"]!;
        this.scopes = scopes;

        InnerHandler = new HttpClientHandler();        
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            accessToken = await GetAccessToken();
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
    
    private async Task<string?> GetAccessToken()
    {
        var client = new HttpClient();

        var request = $"client_id={clientId}&client_secret={clientSecret}&scope={string.Join("%20", scopes)}&grant_type=client_credentials";

        var body = new StringContent(request, Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await client.PostAsync(tokenUri, body);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TokenResult>();

        return result?.access_token;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    // ReSharper disable once InconsistentNaming
    private record TokenResult(string access_token);
    
}