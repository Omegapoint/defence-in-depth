using System;
using System.Linq;

namespace Defence.In.Depth.Domain.Model
{
    public class ProductId
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
                throw new ArgumentException($"Id {id} is not valid.");
            }
        }

        public static bool operator ==(ProductId left, ProductId right)
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

        public static bool operator !=(ProductId left, ProductId right)
        {
            return !(left == right);
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return (ProductId)obj == this; // This works since we also override the == operator.
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
