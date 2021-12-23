using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CompleteWithAllDefenceLayers.Test;

internal class TokenHttpClient
{
    private const string Content = "client_id=m2m&client_secret=secret&scope=products.read&grant_type=client_credentials";
    private readonly Uri tokenUri = new Uri("https://localhost:4000/connect/token");

    private readonly HttpClient client;
    private string? accessToken;

    public TokenHttpClient()
    {
        client = new HttpClient();
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

    private async Task Initialize()
    {
        var client = new HttpClient();

        var body = new StringContent(Content, Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await client.PostAsync(tokenUri, body);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<TokenResult>(content);

        accessToken = result.access_token;
    }

    private class TokenResult
    {
        public string? access_token { get; set; }
    }
}