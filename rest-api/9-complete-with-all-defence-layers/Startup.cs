using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

namespace Defence.In.Depth
{
    public class Startup
    {
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
                    //TODO: validate aud not needed? IdP should handle this if configured properly... 
                    //Is this needed for type vaidation? options.TokenTypeHint = "access_token"; 
                    options.ClientId = "resource1";
                    options.ClientSecret = "secret";
                })
                // Add support for mTLS, from http://docs.identityserver.io/en/latest/topics/mtls.html
                .AddCertificate(options =>
                {
                    options.AllowedCertificateTypes = CertificateTypes.All;
                });

            // Demo 2 - Require Bearer authentication scheme for all requests (including non mvc requests), 
            // even if no other policy has been configured
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .Build();

                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .Build();
            });

            // Demo 3 - Add claims transformation
            services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Demo 1 - Force https, if done in API-gateway (which often terminates TLS) this code could be removed.
            app.UseHttpsRedirection();
            app.UseHsts();
            
            // Demo 1 - If TLS is terminated before our application then we need to handle forwarded headers
            // in order to support certificate bound tokens later on (Demo 2). Note that we this code should be
            // removed if the API does not have an API-gateway in-front (which terminates TLS) 
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            };
            app.UseForwardedHeaders(options);

            app.UseRouting();
            app.UseAuthentication();
            
            // Demo 2 - Add mTLS certificate token binding support, from http://docs.identityserver.io/en/latest/topics/mtls.html
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
