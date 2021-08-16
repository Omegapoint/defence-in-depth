using System;
using System.Linq;
using Defence.In.Depth.Domain.Exceptions;

namespace Defence.In.Depth.Domain.Model
{
    public class MarketId : IDomainPrimitive<string>
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
        
        public static bool operator ==(MarketId left, MarketId right)
        {
            if (ReferenceEquals(null, left))
            {
                return ReferenceEquals(null, right);
            }

            if (ReferenceEquals(null, right))
            {
                return false;
            }

            if (ReferenceEquals(left, right))
            {
                return true;
            }

            return string.Equals(left.Value, right.Value, StringComparison.Ordinal);
        }

        public static bool operator !=(MarketId left, MarketId right)
        {
            return !(left == right);
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return (MarketId)obj == this; // This works since we also override the == operator.
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 23;
                hash = hash * 31 + Value.GetHashCode();

                return hash;
            }
        }
    }
}