using Defence.In.Depth.Domain.Exceptions;
using Defence.In.Depth.Domain.Models;
using Test.Unit.Token.Domain;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class MarketIdTests
{
    [Theory]
    [MemberData(nameof(TestData.InjectionStrings), MemberType = typeof(TestData))]
    public void Constructor_Should_Reject_InvalidData(string id)
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new MarketId(id));
    }

    [Fact]
    public void Constructor_Should_Reject_EmptyData()
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new MarketId(null!));
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new MarketId(string.Empty));
    }
    
    [Theory]
    [InlineData("se")]
    [InlineData("no")]
    [InlineData("fi")]
    [InlineData("FI")]
    public void Constructor_Accept_ValidData(string id)
    {
        Assert.Equal(id, new MarketId(id).Value);   
    }
}