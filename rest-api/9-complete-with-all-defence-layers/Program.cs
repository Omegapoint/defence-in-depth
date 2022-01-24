using Azure.Identity;
using Defence.In.Depth;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;
using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using System.IdentityModel.Tokens.Jwt;


// Demo 1 - The default configuration will on Windows, where IIS is available,
// run Kestrel in-process hosted by the IIS with the IIS ASP.NET Core module as a reverse proxy. 
// On e g Linux it will be hosted out-of process in Kestrel, and you need a reverse proxy like NGINX in front
var builder = WebApplication.CreateBuilder(args);

// Demo 8 - Handle secretes using App Configuration and Key Vault
// Note that MSI needs to be set up and secretes needs to reference key vault
// https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references
var azureAppConfigurationUrl =  builder.Configuration["AzureAppConfiguration:Url"];
if(!string.IsNullOrEmpty(azureAppConfigurationUrl))
{
    builder.WebHost.ConfigureAppConfiguration((_, config) =>
    {
        var credentials = new DefaultAzureCredential();

        config.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(azureAppConfigurationUrl), credentials)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(credentials);
                });
        });
    });
}

// Use some sort of centralized logging service (e g Application Insights) to log all exceptions etc.
// Note that UseDeveloperExceptionPage is built in to the ASP.NET Core 6 default web host builder,
// So the API will display detailed execution messages if Environment.IsDevelopment() returns true. 
builder.Services.AddApplicationInsightsTelemetry();

// We want all claims from the IdP, not filtered or altered by ASP.NET Core
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Demo 2 - This JWT middleware has secure defaults, with validation according to the JWT spec, 
// all we need to do is configure iss and aud, and add valid type.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = "https://localhost:4000";
        options.Audience = "products.api";

        // Note that type validation might need to be done differently depending in token service (IdP).
        options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };

        options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection");
    })
    // Add support for reference-tokens, from https://leastprivilege.com/2020/07/06/flexible-access-token-validation-in-asp-net-core/
    .AddOAuth2Introspection("introspection", options =>
    {
        options.Authority = builder.Configuration["Introspection:Authority"] ?? "https://localhost:4000";
        options.ClientId = builder.Configuration["Introspection:ClientId"] ?? "resource1";
        options.ClientSecret = builder.Configuration["Introspection:ClientSecret"] ?? "secret";
    });

// Demo 2 - Require Bearer authentication scheme for all requests (including non mvc requests), 
// even if no other policy has been configured.
// Enable public endpoints by decorating with the AllowAnonymous attribute.
builder.Services.AddAuthorization(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .Build();

    options.DefaultPolicy = policy;
    options.FallbackPolicy = policy;
});

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IAuditService, LoggerAuditService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddPermissionService();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddControllers();

var app = builder.Build();

// Demo 1 - Force https, this is done in the NGINX reverse proxy.
//app.UseHttpsRedirection();
//app.UseHsts();
    
// Demo 1 - TLS is terminated before our application and we need to handle forwarded headers
// in order to support more advanced features like certificate bound tokens, se e g
// https://docs.duendesoftware.com/identityserver/v5/apis/aspnetcore/confirmation 
// Note that this code should be removed if the API does not have a reverse proxy or API-gateway in-front (which terminates TLS). 
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
    
app.UseEndpoints(endpoints =>
{
    // Demo 2 - Even if we have the fallback policy it is a good practice to set a explicit policy
    // for each mapped route (with RequireAuthorization we apply the Default policy).
    // With this code it takes two mistakes get a public endpoint.
    endpoints
        .MapControllers()
        .RequireAuthorization();
});

app.Run();
