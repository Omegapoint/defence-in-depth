using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace CompleteWithAllDefenceLayers.Tests.System;

public class BaseTests
{
    protected readonly IConfiguration Configuration;

    public BaseTests(ITestOutputHelper output)
    {
        var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "localhost";

        var builder = new ConfigurationBuilder()
            .AddJsonFile("Tests.System/testsettings.json", optional: false)
            .AddJsonFile($"Tests.System/testsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
        
        output.WriteLine("API base uri: {0}", Configuration["BaseAddress:Uri"]);
    }

    protected HttpClient CreateAuthenticatedHttpClient()
    {
        var httpClient = new HttpClient(new ClientCredentialsDelegatingHandler(Configuration));
        httpClient.BaseAddress = new Uri(Configuration["BaseAddress:Uri"]!);

        return httpClient;
    }

    protected HttpClient CreateAuthenticatedHttpClient(string[] scopes)
    {
        var httpClient = new HttpClient(new ClientCredentialsDelegatingHandler(Configuration, scopes));
        httpClient.BaseAddress = new Uri(Configuration["BaseAddress:Uri"]!);

        return httpClient;
    }
    
    protected HttpClient CreateAnonymousHttpClient()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Configuration["BaseAddress:Uri"]!);

        return httpClient;
    }
}