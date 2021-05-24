using System;
using System.Security.Claims;
using Defence.In.Depth.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace Defence.In.Depth.Domain.Services
{
    public class PermissionService : IPermissionService
    {
        public bool CanReadProducts { get; private set; }

        public bool CanWriteProducts { get; private set; }
        
        public MarketId MarketId { get; private set; }

        public PermissionService(IHttpContextAccessor contextAccessor)
        {
            Initialize(contextAccessor.HttpContext?.User);
        }
        
        private static void IfScope(ClaimsPrincipal principal, string scope, Action action)
        {
            if (principal.HasClaim(claim => claim.Type == "scope" && claim.Value == scope))
            {
                action();
            }
        }
        
        private void Initialize(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            
            // This sample will just add hard-coded claims to any authenticated
            // user, but a real example would of course instead use a local
            // account database to get information about what organization and
            // local permissions to add.

            // It is important to honor any scope that affect our domain
            IfScope(principal, "products.read", () => CanReadProducts = true);
            IfScope(principal, "products.write", () => CanWriteProducts = true);

            var market = principal.FindFirstValue("urn:identity:market");
            MarketId = new MarketId(market);
        }
    }
}