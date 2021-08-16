// Source code (with minor changes) is from https://docs.identityserver.io/en/latest/topics/mtls.html
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace Defence.In.Depth
{
    public class ConfirmationValidationMiddlewareOptions
    {
        public string CertificateSchemeName { get; set; } = CertificateAuthenticationDefaults.AuthenticationScheme;
        public string JwtBearerSchemeName { get; set; } = JwtBearerDefaults.AuthenticationScheme;
    }

    // this middleware validate the cnf claim (if present) against the thumbprint of the X.509 client certificate for the current client
    public class ConfirmationValidationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ConfirmationValidationMiddlewareOptions options;

        public ConfirmationValidationMiddleware(RequestDelegate next, ConfirmationValidationMiddlewareOptions options = null)
        {
            this.next = next;
            this.options = options ?? new ConfirmationValidationMiddlewareOptions();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var json = context.User.FindFirst("cnf")?.Value;

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var authenticateResult = await context.AuthenticateAsync(options.CertificateSchemeName);
                    
                    if (!authenticateResult.Succeeded)
                    {
                        await context.ChallengeAsync(options.CertificateSchemeName);
                        return;
                    }

                    var certificate = await context.Connection.GetClientCertificateAsync();

                    if (certificate == null)
                    {
                        throw new InvalidOperationException("Unable to find client certificate");
                    }
                    
                    var thumbprint = Base64UrlTextEncoder.Encode(certificate.GetCertHash(HashAlgorithmName.SHA256));

                    var cnf = JsonDocument.Parse(json).RootElement;

                    var sha256 = cnf.GetString("x5t#S256")?.Trim();

                    if (string.IsNullOrWhiteSpace(sha256) || !thumbprint.Equals(sha256, StringComparison.Ordinal))
                    {
                        await context.ChallengeAsync(options.JwtBearerSchemeName);
                        return;
                    }
                }
            }

            await next(context);
        }
    }
}