using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options => options
    .Connect(
        new Uri(builder.Configuration["AzureAppConfiguration:Url"] ?? string.Empty), 
        new DefaultAzureCredential())
    .ConfigureKeyVault(c => c.SetCredential(new DefaultAzureCredential())));

builder.Services.AddAuthentication()
    .AddJwtBearer(options => {
        // TokenValidationParameters are not currently supported in appsettings.config for .NET 10
        // Note that type validation might differ, depending on token service (IdP)
        options.TokenValidationParameters.ValidTypes = ["at+jwt"];
    });

builder.Services.AddAuthorization(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .Build();

    options.DefaultPolicy = policy;
    options.FallbackPolicy = policy;
});

var app = builder.Build();

// Note that with minimal APIs, UseAuthentication and UseAuthorization is called automatically from AddAuthentication and AddAuthorization.
// See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/middleware?view=aspnetcore-10.0

app.MapGet("/", () => "Hello World!")
            .RequireAuthorization();

app.Run();
