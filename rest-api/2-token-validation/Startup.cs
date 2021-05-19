using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace _2_token_validation
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
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

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();
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
