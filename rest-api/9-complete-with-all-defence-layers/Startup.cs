using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;
using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens.Jwt;

namespace Defence.In.Depth
{
    public class Startup
    {
        public Startup()
        {
            //Demo 3 - We want all claims from the IdP, not filtered or altered by ASP.NET Core
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Demo 1 - Log all excpetions, limit the risk of exposing exeption details by removing UseDeveloperExceptionPage()
            services.AddApplicationInsightsTelemetry();

            // Demo 2 - This JWT middeware has secure defaults, with validation according to the JWT spec, 
            // all we need to do is configure iss and aud, and add validation for token type and binding.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.Authority = "https://demo.identityserver.io";
                    options.Audience = "api";

                    // Note that type validation might need to be done differntly depending in token serivce (IdP).
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };

                    options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection");
                })
                // Add support for reference-tokens, from https://leastprivilege.com/2020/07/06/flexible-access-token-validation-in-asp-net-core/
                .AddOAuth2Introspection("introspection", options =>
                {
                    options.Authority = "https://demo.identityserver.io";
                    options.ClientId = "resource1";
                    options.ClientSecret = "secret";
                    // Note that the client should be configured in such way that introspection is
                    // restricted to the API audience and access tokens, hence no need validate this.
                });

                // Add support for mTLS, from 
                // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-5.0
                // https://docs.identityserver.io/en/latest/topics/mtls.html
                services.AddCertificateForwarding(options =>
                {
                    // header name might be different, based on your nginx config
                    options.CertificateHeader = "X-SSL-CERT";

                    options.HeaderConverter = (headerValue) =>
                    {
                        X509Certificate2 clientCertificate = null;

                        if(!string.IsNullOrWhiteSpace(headerValue))
                        {
                            var bytes = Encoding.UTF8.GetBytes(Uri.UnescapeDataString(headerValue));
                            clientCertificate = new X509Certificate2(bytes);
                        }

                        return clientCertificate;
                    };
                });

            // Demo 2 - Require Bearer authentication scheme for all requests (including non mvc requests), 
            // even if no other policy has been configured
            services.AddAuthorization(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .Build();

                options.DefaultPolicy = policy;
                options.FallbackPolicy = policy;
            });

            // Demo 3 - Add claims transformation
            services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();

            // Demo 7 - Domain driven security
            services.AddTransient<IClaimsTransformation, ClaimsTransformation>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IProductRepository, ProductRepository>();

            services.AddHttpContextAccessor();
            services.AddPermissionService();

            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Demo 1 - Force https, this is done in the API-gateway.
            //app.UseHttpsRedirection();
            //app.UseHsts();
            
            // Demo 1 - TLS is terminated before our application and we need to handle forwarded headers
            // in order to support certificate bound tokens later on (Demo 2). Note that we this code should be
            // removed if the API does not have an API-gateway in-front (which terminates TLS) 
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Demo 2 - Add mTLS support
            app.UseCertificateForwarding();

            app.UseRouting();
            app.UseAuthentication();

            // Demo 2 - Add mTLS certificate token binding support
            app.UseMiddleware<ConfirmationValidationMiddleware>(new ConfirmationValidationMiddlewareOptions
            {
                CertificateSchemeName = CertificateAuthenticationDefaults.AuthenticationScheme,
                JwtBearerSchemeName = JwtBearerDefaults.AuthenticationScheme
            });

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                // Demo 2 - even if we have the fallback policy it is a good practice to set a explicit policy
                // for each mapped route (RequireAuthorization we apply the Default policy)
                endpoints
                    .MapControllers()
                    .RequireAuthorization();
            });
        }
    }
}
