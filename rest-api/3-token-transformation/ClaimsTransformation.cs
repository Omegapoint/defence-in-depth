using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Defence.In.Depth;

internal class ClaimsTransformation : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        await Task.CompletedTask;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var identity = new ClaimsIdentity(principal.Identity);

            // This sample will just add hard-coded claims to any authenticated
            // user, but a real example would of course instead use a local
            // account database or external service to get information about 
            // local permissions to add.
            // Note that in this demo we represent permissions as claims, but in 
            // demo 7 (and 9) we move to a permissions service.

            // It is important to honor any scope that affect our domain
            AddPermissionIfScope(identity, "products.read",  new Claim("urn:permissions:products:read",  "true"));
            AddPermissionIfScope(identity, "products.write", new Claim("urn:permissions:products:write", "true"));

            // Example claim that is related to identity (the sub claim), not scopes.
            identity.AddClaim(new Claim("urn:permissions:market", "se"));

            return new ClaimsPrincipal(identity);
        }

        return principal;
    }

    private void AddPermissionIfScope(ClaimsIdentity identity, string scope, Claim claim)
    {
        if (identity.Claims.Any(c => c.Type == "scope" && c.Value == scope))
        {
            identity.AddClaim(claim);
        }
    }
}