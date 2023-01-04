namespace Defence.In.Depth;

public class RateLimitOptions
{
    public const string JwtPolicyName = "jwt";

    public const string RateLimit = "RateLimit";
    
    public int TokenLimitAnonymous {get;set;}
    public int TokenLimitAuthenticated {get;set;}
    public int QueueLimit {get;set;}
    public double ReplenishmentPeriod {get;set;}
    public int TokensPerPeriod {get;set;}
    public bool AutoReplenishment {get;set;}
}
