using System.Security.Claims;
using Defence.In.Depth.Domain.Model;

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

        // It is important to honor any scope that affect our domain
        IfScope(principal, "products.read", () => CanReadProducts = true);
        IfScope(principal, "products.write", () => CanWriteProducts = true);

        // There is a balance between this class and ClaimsTransformation. In our
        // case, which market a user belongs to could be added in
        // ClaimsTransformation, but you might find that that kind of code is
        // better placed here, inside your domain, especially if it requires an
        // external lookup. In real world scenarios we would most likely lookup
        // market information etc given the identity.
        // Here we have just hard coded the market to the Swedish for all users.        
        MarketId = new MarketId("se");
    }
        
    public bool CanReadProducts { get; private set; }

    public bool CanWriteProducts { get; private set; }
        
    public MarketId MarketId { get; private set; }

    public UserId? UserId { get; private set; }

    public ClientId? ClientId { get; private set; }

    public AuthenticationMethods AuthenticationMethods { get; private set; }

    public bool HasPermissionToMarket(MarketId requestedMarket)
    {
        return string.Equals(MarketId.Value, requestedMarket.Value, StringComparison.OrdinalIgnoreCase);
    } 

    private static void IfScope(ClaimsPrincipal principal, string scope, Action action)
    {
        if (principal.HasClaim(claim => claim.Type == "scope" && claim.Value == scope))
        {
            action();
        }
    }
}