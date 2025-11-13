using Defence.In.Depth;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // TokenValidationParameters not not currently supported in appsettings.config for .NET 7
        // Note that type validation might differ, depending on token serivce (IdP)
        options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
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

builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.Run();