using Defence.In.Depth.Domain.Exceptions;
using Defence.In.Depth.Domain.Models;
using Test.Unit.Token.Domain;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class UserIdTests
{
    [Theory]
    [MemberData(nameof(TestData.InjectionStrings), MemberType = typeof(TestData))]
    public void Constructor_Should_Reject_InvalidData(string id)
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new UserId(id));
    }

    [Fact]
    public void Constructor_Should_Reject_EmptyData()
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new UserId(null!));
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new UserId(string.Empty));
    }
    
    [Theory]
    [InlineData("oilasdfag")]
    [InlineData("p978rtfvs")]
    public void Constructor_Accept_ValidData(string id)
    {
        Assert.Equal(id, new UserId(id).Value);   
    }
}