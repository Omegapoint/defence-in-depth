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

                // Normally a query against database or service
                identity.AddClaim(new Claim("urn:identity:market", "se"));

                return new ClaimsPrincipal(identity);
            }

            return principal;
        }
    }
}