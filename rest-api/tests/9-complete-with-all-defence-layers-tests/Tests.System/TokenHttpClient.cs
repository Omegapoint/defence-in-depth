using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace CompleteWithAllDefenceLayers.Tests.System;

internal class TokenHttpClient
{
    private string requestContent;
    private readonly Uri tokenUri = new Uri("https://localhost:4000/connect/token");

    private readonly HttpClient client;
    private string? accessToken;

    public TokenHttpClient(string scope = "products.read")
    {
        client = new HttpClient();
        requestContent = $"client_id=m2m&client_secret=secret&scope={scope}&grant_type=client_credentials";
    }

    public async Task<HttpResponseMessage> GetAsync(Uri requestUri)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            await Initialize();
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await client.GetAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PutAsync(Uri requestUri)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            await Initialize();
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await client.PutAsync(requestUri, null);
    }

    private async Task Initialize()
    {
        var innerClient = new HttpClient();

        var body = new StringContent(requestContent, Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await innerClient.PostAsync(tokenUri, body);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<TokenResult>(responseContent);

        accessToken = result?.access_token;
    }

    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedAutoPropertyAccessor.Local
    private class TokenResult
    {
        public string? access_token { get; set; }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore UnusedAutoPropertyAccessor.Local
}