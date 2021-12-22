namespace Defence.In.Depth.Domain.Services; 

public static class ClaimSettings
{
    //Authorization server scopes
    public const string ProductsRead = "products.read";
    public const string ProductsWrite = "products.write";

    //Authorization server claims
    public const string UrnIdentityMarket = "urn:identity:market";
    public const string Sub = "sub";
    public const string ClientId = "client_id";
    public const string AMR = "amr";
}

