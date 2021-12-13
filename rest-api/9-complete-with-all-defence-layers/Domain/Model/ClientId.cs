using System;
using System.Linq;
using Defence.In.Depth.Domain.Exceptions;

namespace Defence.In.Depth.Domain.Model
{
    public record ClientId : IDomainPrimitive<string>
    {
        public ClientId(string subject)
        {
            AssertValidClientId(subject);

            Value = subject;
        }

        public string Value { get; }

        public static bool IsValidClientId(string clientId)
        {
            return !string.IsNullOrEmpty(clientId) && clientId.Length < 10 && clientId.All(char.IsLetterOrDigit);
        }

        public static void AssertValidClientId(string clientId)
        {
            if (!IsValidClientId(clientId))
            {
                throw new DomainPrimitiveArgumentException<string>(clientId);
            }
        }
    }
}