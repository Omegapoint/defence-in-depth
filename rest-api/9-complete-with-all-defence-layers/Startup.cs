using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;
using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace Defence.In.Depth;

public class Startup
{
    public Startup()
    {
        //Demo 3 - We want all claims from the IdP, not filtered or altered by ASP.NET Core
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
    }
        
    public void ConfigureServices(IServiceCollection services)
    {
        // Use some sort of centralized logging service (e g Application Insights) to log all excpetions etc.
        // Note that UseDeveloperExceptionPage is built in to the ASP.NET Core 6 default web host builder,
        // So the API will display detailed execption messages if Environment.IsDevelopment() returns true. 
        services.AddApplicationInsightsTelemetry();

        // Demo 2 - This JWT middeware has secure defaults, with validation according to the JWT spec, 
        // all we need to do is configure iss and aud, and add validation for token type and binding.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.Authority = "https://localhost:4000";
                options.Audience = "products.api";

                // Note that type validation might need to be done differntly depending in token serivce (IdP).
                options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };

                options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection");
            })
            // Add support for reference-tokens, from https://leastprivilege.com/2020/07/06/flexible-access-token-validation-in-asp-net-core/
            .AddOAuth2Introspection("introspection", options =>
            {
                options.Authority = "https://localhost:4000";
                options.ClientId = "resource1";
                options.ClientSecret = "secret";
                // Note that the client should be configured in such way that introspection is
                // restricted to the API audience and access tokens, hence no need validate this.
            });

        // Demo 2 - Require Bearer authentication scheme for all requests (including non mvc requests), 
        // even if no other policy has been configured.
        // Enable public endpoints by decorating with the AllowAnonymous attribute.
        services.AddAuthorization(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build();

            options.DefaultPolicy = policy;
            options.FallbackPolicy = policy;
        });

        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IAuditService, LoggerAuditService>();

        services.AddHttpContextAccessor();
        services.AddPermissionService();

        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Demo 1 - Force https, this is done in the NGINX reverse proxy.
        //app.UseHttpsRedirection();
        //app.UseHsts();
            
        // Demo 1 - TLS is terminated before our application and we need to handle forwarded headers
        // in order to support certificate bound tokens later on (as part of Demo 2). 
        // Note that we this code should be removed if the API does not have a reverse proxy or API-gateway in-front (which terminates TLS). 
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
    }
}