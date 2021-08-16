using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Defence.In.Depth
{
    internal class ClaimsTransformation : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            await Task.CompletedTask;

            if (principal.Identity?.IsAuthenticated == true)
            {
                var identity = new ClaimsIdentity(principal.Identity);

                // There is a balance between this class and PermissionService.  As a
                // general rule of thumb, limit this class to only deal with identity.
                // Adding a claim for which market a user belongs to might belong here.
                identity.AddClaim(new Claim("urn:identity:market", "se"));

                return new ClaimsPrincipal(identity);
            }

            return principal;
        }
    }
}