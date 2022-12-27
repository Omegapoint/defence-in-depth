using Defence.In.Depth.Domain.Exceptions;
using Defence.In.Depth.Domain.Models;
using Test.Unit.Token.Domain;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ClientIdTests
{
    [Theory]
    [MemberData(nameof(TestData.InjectionStrings), MemberType = typeof(TestData))]
    public void Constructor_Should_Reject_InvalidData(string id)
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ClientId(id));
    }

    [Fact]
    public void Constructor_Should_Reject_EmptyData()
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ClientId(null!));
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ClientId(string.Empty));
    }
    
    [Theory]
    [InlineData("abcdefghi")]
    [InlineData("123456789")]
    public void Constructor_Accept_ValidData(string id)
    {
        Assert.Equal(id, new ClientId(id).Value);   
    }
}