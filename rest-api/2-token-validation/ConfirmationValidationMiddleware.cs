//From http://docs.identityserver.io/en/latest/topics/mtls.html

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2_token_validation
{
    public class ConfirmationValidationMiddlewareOptions
    {
        public string CertificateSchemeName { get; set; } = CertificateAuthenticationDefaults.AuthenticationScheme;
        public string JwtBearerSchemeName { get; set; } = JwtBearerDefaults.AuthenticationScheme;
    }

    // this middleware validate the cnf claim (if present) against the thumbprint of the X.509 client certificate for the current client
    public class ConfirmationValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConfirmationValidationMiddlewareOptions _options;

        public ConfirmationValidationMiddleware(RequestDelegate next, ConfirmationValidationMiddlewareOptions options = null)
        {
            _next = next;
            _options = options ?? new ConfirmationValidationMiddlewareOptions();
        }

        public async Task Invoke(HttpContext ctx)
        {
            if (ctx.User.Identity.IsAuthenticated)
            {
                var cnfJson = ctx.User.FindFirst("cnf")?.Value;
                if (!String.IsNullOrWhiteSpace(cnfJson))
                {
                    var certResult = await ctx.AuthenticateAsync(_options.CertificateSchemeName);
                    if (!certResult.Succeeded)
                    {
                        await ctx.ChallengeAsync(_options.CertificateSchemeName);
                        return;
                    }

                    var certificate = await ctx.Connection.GetClientCertificateAsync();
                    var thumbprint = Base64UrlTextEncoder.Encode(certificate.GetCertHash(HashAlgorithmName.SHA256));

                    var cnf = JsonDocument.Parse(cnfJson).RootElement;
                    var sha256 = cnf.GetString("x5t#S256").Trim();

                    if (string.IsNullOrWhiteSpace(sha256) ||
                        !thumbprint.Equals(sha256, StringComparison.Ordinal))
                    {
                        await ctx.ChallengeAsync(_options.JwtBearerSchemeName);
                        return;
                    }
                }
            }

            await _next(ctx);
        }
    }
}