using System;
using System.Linq;
using Defence.In.Depth.Domain.Exceptions;

namespace Defence.In.Depth.Domain.Model;

public record MarketId : IDomainPrimitive<string>
{
    public MarketId(string id)
    {
        AssertValidId(id);

        Value = id;
    }

    public string Value { get; }

    public static bool IsValidId(string id)
    {
        var allowList = new[] { "se", "no", "fi" };
            
        return allowList.Contains(id, StringComparer.OrdinalIgnoreCase);
    }

    public static void AssertValidId(string id)
    {
        if (!IsValidId(id))
        {
            throw new DomainPrimitiveArgumentException<string>(id);
        }
    }
}