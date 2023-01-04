using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace Defence.In.Depth;

public static class WebApplicationBuilderExtensions
{
    //From https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-7.0#limiter-with-authorization
    public static WebApplicationBuilder AddRateLimitPolicies(this WebApplicationBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var options = new RateLimitOptions();
        builder.Configuration.GetSection(RateLimitOptions.RateLimit).Bind(options);

        builder.Services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            limiterOptions.AddPolicy(policyName: RateLimitOptions.JwtPolicyName, partitioner: httpContext =>
            {
                var accessToken = httpContext.Features.Get<IAuthenticateResultFeature>()?
                                    .AuthenticateResult?.Properties?.GetTokenValue("access_token")?.ToString()
                                ?? string.Empty;

                if (!StringValues.IsNullOrEmpty(accessToken))
                {
                    return RateLimitPartition.GetTokenBucketLimiter(accessToken, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = options.TokenLimitAuthenticated,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = options.QueueLimit,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(options.ReplenishmentPeriod),
                            TokensPerPeriod = options.TokensPerPeriod,
                            AutoReplenishment = options.AutoReplenishment
                        });
                }

                return RateLimitPartition.GetTokenBucketLimiter("Anon", _ =>
                    new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = options.TokenLimitAnonymous,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = options.QueueLimit,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(options.ReplenishmentPeriod),
                        TokensPerPeriod = options.TokensPerPeriod,
                        AutoReplenishment = true
                    });
            });
        });

        return builder;
    }
}
