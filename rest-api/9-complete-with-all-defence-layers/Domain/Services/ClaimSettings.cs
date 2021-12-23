namespace Defence.In.Depth.Domain.Services; 

public static class ClaimSettings
{
    //Authorization server claim types
    public const string Sub = "sub";
    public const string ClientId = "client_id";
    public const string Scope = "scope";
    public const string AMR = "amr";
    public const string UrnIdentityMarket = "urn:identity:market";

    //Authorization server AMR values 
    public const string AuthenticationMethodPassword = "pwd";

    //Authorization server scope values
    public const string ProductsRead = "products.read";
    public const string ProductsWrite = "products.write";
}

