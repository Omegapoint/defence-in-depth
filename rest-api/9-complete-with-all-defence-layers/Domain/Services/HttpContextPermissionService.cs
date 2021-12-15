using System;
using System.Linq;
using System.Security.Claims;
using Defence.In.Depth.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace Defence.In.Depth.Domain.Services;

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

        // There is a balance between this class and ClaimsTransformation. In our
        // case, which market a user belongs to could be added in
        // ClaimsTransformation, but you might find that that kind of code is
        // better placed here, inside your domain, especially if it requires an
        // external lookup. In real world scenarios we would most likely lookup
        // market information etc given the identity.
        var market = principal.FindFirstValue("urn:identity:market");            
        MarketId = market == null ? null : new MarketId(market);

        var sub = principal.FindFirstValue("sub");
        UserId = sub == null ? null : new UserId(sub);

        var clientId = principal.FindFirstValue("client_id");
        ClientId = clientId == null ? null : new ClientId(clientId);
            
        AuthenticationMethods = principal.Claims
            .Where(c => c.Type == "amr")
            .Select(claim => claim.Value switch
            {
                "pwd" => AuthenticationMethods.Password,
                    _ => AuthenticationMethods.Unknown
            })
            .Aggregate(AuthenticationMethods.None, (prev , next) => prev | next);
    }
        
    public bool CanReadProducts { get; private set; }

    public bool CanWriteProducts { get; private set; }
        
    public MarketId MarketId { get; private set; }

    public UserId UserId { get; private set; }

    public ClientId ClientId { get; private set; }

    public AuthenticationMethods AuthenticationMethods { get; private set; }

    private static void IfScope(ClaimsPrincipal principal, string scope, Action action)
    {
        if (principal.HasClaim(claim => claim.Type == "scope" && claim.Value == scope))
        {
            action();
        }
    }
}