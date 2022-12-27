using Defence.In.Depth.Domain.Exceptions;

namespace Defence.In.Depth.Domain.Models;

public record ProductId : IDomainPrimitive<string>
{
    public ProductId(string id)
    {
        AssertValidId(id);

        Value = id;
    }

    public string Value { get; }

    public static bool IsValidId(string id)
    {
        return !string.IsNullOrEmpty(id) && id.Length < 10 && id.All(char.IsLetterOrDigit);
    }

    public static void AssertValidId(string id)
    {
        if (!IsValidId(id))
        {
            throw new DomainPrimitiveArgumentException<string>(id);
        }
    }
}