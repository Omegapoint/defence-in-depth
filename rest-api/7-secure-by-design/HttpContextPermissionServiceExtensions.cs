using System;
using Defence.In.Depth.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Defence.In.Depth
{
    public static class HttpContextPermissionServiceExtensions
    {
        public static IServiceCollection AddPermissionService(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Since the HttpContextPermissionService class has state originating
            // from the HttpContext of the request, the lifetime of the service MUST
            // be scoped, i.e., we need a new instance at every client request
            // (connection).
            services.AddScoped<IPermissionService, HttpContextPermissionService>();

            return services;
        }
    }
}