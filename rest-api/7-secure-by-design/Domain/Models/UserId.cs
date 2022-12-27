using Defence.In.Depth.Domain.Exceptions;

namespace Defence.In.Depth.Domain.Models;

public record UserId : IDomainPrimitive<string>
{
    public UserId(string subject)
    {
        AssertValidUserId(subject);

        Value = subject;
    }

    public string Value { get; }

    public static bool IsValidUserId(string subject)
    {
        return !string.IsNullOrEmpty(subject) && subject.Length < 10 && subject.All(char.IsLetterOrDigit);
    }

    public static void AssertValidUserId(string subject)
    {
        if (!IsValidUserId(subject))
        {
            throw new DomainPrimitiveArgumentException<string>(subject);
        }
    }
}