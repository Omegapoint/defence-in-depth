using Defence.In.Depth.Domain.Exceptions;
using Defence.In.Depth.Domain.Models;
using Test.Unit.Token.Domain;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductIdTests
{
    [Theory]
    [MemberData(nameof(TestData.InjectionStrings), MemberType = typeof(TestData))]
    [MemberData(nameof(InvalidIds))]
    public void Constructor_Should_Reject_InvalidData(string id)
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(id));
    }

    [Fact]
    public void Constructor_Should_Reject_EmptyData()
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(null!));
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(string.Empty));
    }

    [Theory]
    [InlineData("abcdefghi")]
    [InlineData("123456789")]
    public void Constructor_Accept_ValidData(string id)
    {
        Assert.Equal(id, new ProductId(id).Value);   
    }

    public static IEnumerable<object[]> InvalidIds => new[]
    {
        new object[] { "no spaces" },
        new object[] { "thisisanidthatistoolong" },
        new object[] { "#" }
    };
}