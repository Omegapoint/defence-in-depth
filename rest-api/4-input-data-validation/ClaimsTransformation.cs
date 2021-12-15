using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Defence.In.Depth;

internal class ClaimsTransformation : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        await Task.CompletedTask;

        if (principal.Identity.IsAuthenticated)
        {
            var identity = new ClaimsIdentity(principal.Identity);

            // This sample will just add hard-coded claims to any authenticated
            // user, but a real example would of course instead use a local
            // account database or external service to get information about 
            // what organization and local permissions to add.
            // Note that in this demo we represent permissions as claims, but in 
            // demo 7 (and 9) we move to a permissions service.

            // It is important to honor any scope that affect our domain
            AddPermissionIfScope(identity, "products.read",  new Claim("urn:permission:product:read",  "true"));
            AddPermissionIfScope(identity, "products.write", new Claim("urn:permission:product:write", "true"));

            // Example claim that is related to identity (the sub claim), not scopes.
            identity.AddClaim(new Claim("urn:permission:market", "se"));

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