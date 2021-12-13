using System.IdentityModel.Tokens.Jwt;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Defence.In.Depth
{
    public class Startup
    {
        public Startup()
        {
            // We want all claims from the IdP, not filtered or altered by ASP.NET Core
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IClaimsTransformation, ClaimsTransformation>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IAuditService, LoggerAuditService>();

            services.AddHttpContextAccessor();
            services.AddPermissionService();
    
            services.AddControllers();

            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.Authority = "https://localhost:4000";
                    options.Audience = "products.api";

                    options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                });
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
