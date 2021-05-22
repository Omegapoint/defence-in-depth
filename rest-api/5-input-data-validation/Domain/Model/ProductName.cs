using System.Linq;
using Defence.In.Depth.Domain.Exceptions;

namespace Defence.In.Depth.Domain.Model
{
    public class ProductName
    {
        public ProductName(string name)
        {
            AssertValidTerm(name);

            Value = name;
        }

        public string Value { get; }

        public static bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.Length < 10 && (name.All(char.IsLetterOrDigit) || name.All(char.IsWhiteSpace));
        }

        public static void AssertValidTerm(string name)
        {
            if (!IsValidName(name))
            {
                throw new InvalidProductNameArgumentException($"The product name {name} is not valid.");
            }
        }
    }
}