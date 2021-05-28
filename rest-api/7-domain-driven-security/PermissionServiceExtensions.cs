using System;
using Defence.In.Depth.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Defence.In.Depth
{
    public static class PermissionServiceExtensions
    {
        public static IServiceCollection AddPermissionService(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // The lifetime of the permission service MUST be scoped, i.e. we need a new
            // instance at every request.  Any other configuration is a security vulnerability.
            services.AddScoped<IPermissionService, PermissionService>();

            return services;
        }
    }
}