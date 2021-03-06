using Defence.In.Depth.Domain.Exceptions;
using Defence.In.Depth.Domain.Model;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductIdTests
{
    [Theory]
    [MemberData(nameof(IdInjection))]
    [MemberData(nameof(InvalidIds))]
    public void ProductId_Should_Reject(string id)
    {
        Assert.False(ProductId.IsValidId(id));
    }

    [Theory]
    [MemberData(nameof(IdInjection))]
    [MemberData(nameof(InvalidIds))]
    public void ProductId_Constructor_ShouldThrow(string id)
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(id));
    }

    [Theory]
    [MemberData(nameof(ValidIds))]
    public void ProductId_Constructor_ShouldNotThrow(string id)
    {
        var product = new ProductId(id);
        Assert.True(product.Value == id);
    }

    public static IEnumerable<object[]> IdInjection => new[]
    {
            new object[] { "<script>" },
            new object[] { "'1==1" },
            new object[] { "--sql" }
        };

    public static IEnumerable<object[]> InvalidIds => new[]
    {
            new object[] { "" },
            new object[] { "no spaces" },
            new object[] { "thisisanidthatistoolong" },
            new object[] { "#" }
        };

    public static IEnumerable<object[]> ValidIds => new[]
    {
            new object[] { "abcdefghi" },
            new object[] { "123456789" },
            new object[] { "dhgf54" }
        };
}