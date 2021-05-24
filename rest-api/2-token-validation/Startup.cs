using IdentityModel.AspNetCore.AccessTokenValidation;
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
        private const string IntrospectionScheme = "introspection";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.Authority = "https://demo.identityserver.io";
                    options.Audience = "api";

                    // Note that type validation might differ, depending on token serivce (IdP)
                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };

                    options.ForwardDefaultSelector = Selector.ForwardReferenceToken(IntrospectionScheme);
                })
                // Add support for reference-tokens, from https://leastprivilege.com/2020/07/06/flexible-access-token-validation-in-asp-net-core/
                .AddOAuth2Introspection(IntrospectionScheme, options =>
                {
                    options.Authority = "https://demo.identityserver.io";
                    options.ClientId = "resource1";
                    options.ClientSecret = "secret";

                })
                // Add support for mTLS, from http://docs.identityserver.io/en/latest/topics/mtls.html
                .AddCertificate(options =>
                {
                    options.AllowedCertificateTypes = CertificateTypes.All;
                });

            services.AddAuthorization(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .Build();

                options.DefaultPolicy = policy;
                options.FallbackPolicy = policy;
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            };

            app.UseForwardedHeaders(options);

            app.UseAuthentication();

            app.UseMiddleware<ConfirmationValidationMiddleware>(new ConfirmationValidationMiddlewareOptions
            {
                CertificateSchemeName = CertificateAuthenticationDefaults.AuthenticationScheme,
                JwtBearerSchemeName = JwtBearerDefaults.AuthenticationScheme
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .MapControllers()
                    .RequireAuthorization();
            });
        }
    }
}
