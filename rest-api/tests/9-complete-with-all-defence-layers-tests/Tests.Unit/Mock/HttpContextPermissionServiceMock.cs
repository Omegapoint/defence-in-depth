using System.Security.Claims;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Microsoft.AspNetCore.Http;

namespace CompleteWithAllDefenceLayers.Tests.Unit.Mock;

public class HttpContextPermissionServiceMock : IPermissionService
{
    public HttpContextPermissionServiceMock(IHttpContextAccessor contextAccessor)
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
            .Aggregate(AuthenticationMethods.None, (prev, next) => prev | next);

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

public class HttpContextAccessorMock : IHttpContextAccessor
{
    private readonly HttpContext testContext;

    public HttpContextAccessorMock(ClaimsIdentity identity)
    {
        testContext = new DefaultHttpContext();
        testContext.User.AddIdentity(identity);
    }
    public HttpContext? HttpContext { get => testContext; set => throw new NotImplementedException(); }
}
