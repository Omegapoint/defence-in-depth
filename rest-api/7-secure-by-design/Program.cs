using Defence.In.Depth;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Endpoints;
using Defence.In.Depth.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IAuditService, LoggerAuditService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddPermissionService();

var app = builder.Build();

app.RegisterProductEndpoints();

app.Run();