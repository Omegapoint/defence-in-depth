using Defence.In.Depth.Domain.Exceptions;
using Defence.In.Depth.Domain.Models;
using Test.Unit.Token.Domain;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductNameTests
{
    [Fact]
    public void Constructor_Should_Reject_InvalidData()
    {
        var name = new string('a', 201); // Too long
        
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductName(name));
    }

    [Fact]
    public void Constructor_Should_Reject_EmptyData()
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductName(null!));
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductName(string.Empty));
    }
    
    [Theory]
    [InlineData("My product name")]
    [InlineData("Best product ever")]
    [MemberData(nameof(TestData.StrangeNames), MemberType = typeof(TestData))]
    public void Constructor_Accept_ValidData(string id)
    {
        Assert.Equal(id, new ProductName(id).Value);   
    }
}