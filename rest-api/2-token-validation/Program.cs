using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
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

builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.Run();