using System;
using System.Security.Claims;
using Defence.In.Depth.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace Defence.In.Depth.Domain.Services
{
    public class PermissionService : IPermissionService
    {
        public PermissionService(IHttpContextAccessor contextAccessor)
        {
            var principal = contextAccessor.HttpContext?.User;

            if (principal == null)
            {
                if (contextAccessor.HttpContext == null)
                {
                    throw new ArgumentException("HTTP Context is null", nameof(contextAccessor));
                }
                
                throw new ArgumentException("User object is null", nameof(contextAccessor));
            }
            
            // It is important to honor any scope that affect our domain
            IfScope(principal, "products.read", () => CanReadProducts = true);
            IfScope(principal, "products.write", () => CanWriteProducts = true);

            // This sample will just add hard-coded claims to any authenticated
            // user, but a real example would use a local database or API to get
            // information about what market and local permissions to add.
            var market = principal.FindFirstValue("urn:identity:market");
            
            MarketId = new MarketId(market);
        }
        
        public bool CanReadProducts { get; private set; }

        public bool CanWriteProducts { get; private set; }
        
        public MarketId MarketId { get; private set; }
        
        private static void IfScope(ClaimsPrincipal principal, string scope, Action action)
        {
            if (principal.HasClaim(claim => claim.Type == "scope" && claim.Value == scope))
            {
                action();
            }
        }
    }
}