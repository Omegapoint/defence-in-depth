using Defence.In.Depth.Domain.Exceptions;

namespace Defence.In.Depth.Domain.Model;

public record ProductName : IDomainPrimitive<string>
{
    public ProductName(string name)
    {
        AssertValidName(name);

        Value = name;
    }

    public string Value { get; }

    public static bool IsValidName(string name)
    {
        return !string.IsNullOrEmpty(name) && name.Length < 20 && (name.All(char.IsLetterOrDigit) || name.All(char.IsWhiteSpace));
    }

    public static void AssertValidName(string name)
    {
        if (!IsValidName(name))
        {
            throw new DomainPrimitiveArgumentException<string>(name);
        }
    }
}