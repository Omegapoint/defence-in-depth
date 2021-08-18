// Source code (with minor JSON changes) is from 
// https://docs.duendesoftware.com/identityserver/v5/apis/aspnetcore/confirmation/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace Defence.In.Depth
{
    // this middleware validate the cnf claim (if present) against the thumbprint of the X.509 client certificate for the current client
    public class ConfirmationValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ConfirmationValidationMiddlewareOptions _options;

        public ConfirmationValidationMiddleware(
            RequestDelegate next, 
            ILogger<ConfirmationValidationMiddlewareOptions> logger, 
            ConfirmationValidationMiddlewareOptions options = null)
        {
            _next = next;
            _logger = logger;
            _options ??= new ConfirmationValidationMiddlewareOptions();
        }

        public async Task Invoke(HttpContext ctx)
        {
            if (ctx.User.Identity.IsAuthenticated)
            {
                // read the cnf claim from the validated token
                var cnfJson = ctx.User.FindFirst("cnf")?.Value;
                if (!String.IsNullOrWhiteSpace(cnfJson))
                {
                    // if present, make sure a valid certificate was presented as well
                    var certResult = await ctx.AuthenticateAsync(_options.CertificateSchemeName);
                    if (!certResult.Succeeded)
                    {
                        await ctx.ChallengeAsync(_options.CertificateSchemeName);
                        return;
                    }

                    // get access to certificate from transport
                    var certificate = await ctx.Connection.GetClientCertificateAsync();
                    var thumbprint = Base64UrlTextEncoder.Encode(certificate.GetCertHash(HashAlgorithmName.SHA256));
                    
                    // retrieve value of the thumbprint from cnf claim (original code use Newtonsoft package)
                    //var cnf = JObject.Parse(cnfJson);
                    //var sha256 = cnf.Value<string>("x5t#S256");
                    var cnf = JsonDocument.Parse(cnfJson).RootElement;
                    var sha256 = cnf.GetString("x5t#S256")?.Trim();

                    // compare thumbprint claim with thumbprint of current TLS client certificate
                    if (String.IsNullOrWhiteSpace(sha256) ||
                        !thumbprint.Equals(sha256, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogError("certificate thumbprint does not match cnf claim.");
                        await ctx.ChallengeAsync(_options.JwtBearerSchemeName);
                        return;
                    }
                    
                    _logger.LogDebug("certificate thumbprint matches cnf claim.");
                }
            }

            await _next(ctx);
        }
    }

    public class ConfirmationValidationMiddlewareOptions
    {
        public string CertificateSchemeName { get; set; } = CertificateAuthenticationDefaults.AuthenticationScheme;
        public string JwtBearerSchemeName { get; set; } = JwtBearerDefaults.AuthenticationScheme;
    }
}