using System.Security.Claims;
using Defence.In.Depth.Domain.Models;

namespace Defence.In.Depth.Domain.Services;

public class HttpContextPermissionService : IPermissionService
{
    // There is a balance between the PermissionService and the ClaimsTransformer.
    // In our case we have moved all code from the ClaimsTransformer to this class,
    // to keep all permission logic in this class, independent of other componentes.
    // Thus, it is the Permission service responsibility to incapsulate token specific 
    // details like custom scope values etc, so we don´t get dependencies to token 
    // formats and protocol details spread out in our code.
    //
    // But note that depending on token format there are cases where we might want 
    // to use a ClaimsTransformer in addition to this class.
    //
    // Also note that in this repo we have placed the PermissionService in our bussiness
    // domain, for other scenarios it might be more appropriate to move this to a
    // subdomain etc. The important thing is that the ProductService requires
    // complete access control, that this is a mandatory part of our core bussiness domain.   
 
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
        
        var sub = principal.FindFirstValue(ClaimSettings.Sub);
        UserId = sub == null ? null : new UserId(sub);

        var clientId = principal.FindFirstValue(ClaimSettings.ClientId);
        ClientId = clientId == null ? null : new ClientId(clientId);
            
        AuthenticationMethods = principal.Claims
            .Where(c => c.Type == ClaimSettings.Amr)
            .Select(claim => claim.Value switch
            {
                ClaimSettings.AuthenticationMethodPassword => AuthenticationMethods.Password,
                ClaimSettings.AuthenticationMethodMFA => AuthenticationMethods.MFA,
                    _ => AuthenticationMethods.Unknown
            })
            .Aggregate(AuthenticationMethods.None, (prev , next) => prev | next);

        // It is important to honor any scope that affect our domain
        IfScope(principal, ClaimSettings.ProductsRead, () => CanReadProducts = true);
        IfScope(principal, ClaimSettings.ProductsWrite, () => CanWriteProducts = true);

        // In real world scenarios we would most likely lookup market information etc 
        // given the identity of the user.
        // Here we have just hard coded the market to the Swedish for all users.        
        MarketId = new MarketId("se");

        // Roles and groups we sometimes get from the IdP as claims in our token 
        // and then we might just need to transform them to fit our model. Or we
        // need to look up roles from a user store, like Azure AD or our own repository.
        // Here we have just hard coded the user role to a role with high privileges.
        UserRoles = UserRoles.ProductManager;
    }
        
    public bool CanReadProducts { get; private set; }

    public bool CanWriteProducts { get; private set; }

    // This demonstrates that permissions can be based on both claims about the user 
    // and the client scopes.
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
        if (principal.HasClaim(claim => claim.Type == ClaimSettings.Scope && claim.Value == scope))
        {
            action();
        }
    }

    private UserRoles UserRoles { get; set; }
}