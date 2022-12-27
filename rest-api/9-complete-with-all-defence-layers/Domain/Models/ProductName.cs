using Defence.In.Depth.Domain.Exceptions;

namespace Defence.In.Depth.Domain.Models;

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
        // Names are very hard to restrict, but we at least limit the size.
        //
        // See also https://stackoverflow.com/q/20958
        return
            !string.IsNullOrEmpty(name) &&
            name.Length <= 200;
    }

    public static void AssertValidName(string name)
    {
        if (!IsValidName(name))
        {
            throw new DomainPrimitiveArgumentException<string>(name);
        }
    }
}