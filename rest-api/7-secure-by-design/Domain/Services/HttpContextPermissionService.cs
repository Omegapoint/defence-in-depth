using System;
using System.Security.Claims;
using Defence.In.Depth.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace Defence.In.Depth.Domain.Services
{
    public class HttpContextPermissionService : IPermissionService
    {
        public HttpContextPermissionService(IHttpContextAccessor contextAccessor)
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

            // There is a balance between this class and ClaimsTransformation.  In
            // our case, which market a user belongs to is added in
            // ClaimsTransformation, but you might find that that kind of code is
            // better placed here, inside your domain.
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