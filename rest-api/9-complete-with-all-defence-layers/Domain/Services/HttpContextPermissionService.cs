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
                "mfa" => AuthenticationMethods.MFA,
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

        // Roles and groups we sometimes get from the IdP as claims in our token 
        // and then we might just need to transform them to fit our model. Or we
        // need to look up roles from a user store, like Azure AD or our own repository.
        // Here we have just hard coded the user role to a role with high privileges.
        // Note that often permissions are based on both the user and the client scopes,
        // as for CanDoHighPrivilegeOperations. 
        UserRoles = UserRoles.ProductManager;
    }
        
    public bool CanReadProducts { get; private set; }

    public bool CanWriteProducts { get; private set; }

    public bool CanDoHighPrivilegeOperations => (
        UserRoles == UserRoles.ProductManager &&  
        CanWriteProducts &&
        AuthenticationMethods == AuthenticationMethods.MFA);
        
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

    private UserRoles UserRoles { get; set; }
}