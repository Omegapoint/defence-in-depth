using System;
using System.Linq;
using Defence.In.Depth.Domain.Exceptions;

namespace Defence.In.Depth.Domain.Model;

public record MarketId : IDomainPrimitive<string>
{
    public MarketId(string countryCode)
    {
        AssertValidCountryCode(countryCode);

        Value = countryCode;
    }

    public string Value { get; }

    public static bool IsValidCountryCode(string name)
    {
        var allowList = new[] { "SE", "NO", "FI" }; // ISO 3166-1 alpha-2 codes
            
        return allowList.Contains(name, StringComparer.OrdinalIgnoreCase);
    }

    public static void AssertValidCountryCode(string countryCode)
    {
        if (!IsValidCountryCode(countryCode))
        {
            throw new DomainPrimitiveArgumentException<string>(countryCode);
        }
    }
}