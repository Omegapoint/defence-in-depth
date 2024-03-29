using System.Security.Claims;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class HttpContextPermissionServiceTests
{  
    [Fact]
    public void HasAllClaimsForSE_GivesAllPermissionForSE()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.Amr, ClaimSettings.AuthenticationMethodPassword),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsWrite)
        };

        var httpContextPermissionService = CreateSut(claims);

        Assert.Equal(new UserId("user1"), httpContextPermissionService.UserId);
        Assert.Equal(new ClientId("client1"), httpContextPermissionService.ClientId);        
        Assert.Equal(AuthenticationMethods.Password, httpContextPermissionService.AuthenticationMethods);
        Assert.True(httpContextPermissionService.CanReadProducts);
        Assert.True(httpContextPermissionService.CanWriteProducts);
        Assert.Equal(new MarketId("se"), httpContextPermissionService.MarketId);
        Assert.True(httpContextPermissionService.HasPermissionToMarket(new MarketId("se")));
    }

    [Fact]
    public void HasAllClaims_ButRead_DeniesRead()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.Amr, "pwd"),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsWrite)
        };

        var httpContextPermissionService = CreateSut(claims);
        
        Assert.Equal(new UserId("user1"), httpContextPermissionService.UserId);
        Assert.Equal(new ClientId("client1"), httpContextPermissionService.ClientId);        
        Assert.Equal(AuthenticationMethods.Password, httpContextPermissionService.AuthenticationMethods);
        Assert.False(httpContextPermissionService.CanReadProducts);
        Assert.True(httpContextPermissionService.CanWriteProducts);
        Assert.Equal(new MarketId("se"), httpContextPermissionService.MarketId);
        Assert.True(httpContextPermissionService.HasPermissionToMarket(new MarketId("se")));
    }

    [Fact]
    public void HasAllClaims_ButWrite_DeniesWrite()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.Amr, "pwd"),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead)
        };
        
        var httpContextPermissionService = CreateSut(claims);
        
        Assert.Equal(new UserId("user1"), httpContextPermissionService.UserId);
        Assert.Equal(new ClientId("client1"), httpContextPermissionService.ClientId);        
        Assert.Equal(AuthenticationMethods.Password, httpContextPermissionService.AuthenticationMethods);
        Assert.True(httpContextPermissionService.CanReadProducts);
        Assert.False(httpContextPermissionService.CanWriteProducts);
        Assert.Equal(new MarketId("se"), httpContextPermissionService.MarketId);
        Assert.True(httpContextPermissionService.HasPermissionToMarket(new MarketId("se")));
    }

    [Fact]
    public void HasAllClaims_ButNoPermissionForMarketNO()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.Amr, "pwd"),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead)
        };
        
        var httpContextPermissionService = CreateSut(claims);
        
        Assert.Equal(new UserId("user1"), httpContextPermissionService.UserId);
        Assert.Equal(new ClientId("client1"), httpContextPermissionService.ClientId);        
        Assert.Equal(AuthenticationMethods.Password, httpContextPermissionService.AuthenticationMethods);
        Assert.True(httpContextPermissionService.CanReadProducts);
        Assert.False(httpContextPermissionService.CanWriteProducts);
        Assert.Equal(new MarketId("se"), httpContextPermissionService.MarketId);
        Assert.True(httpContextPermissionService.HasPermissionToMarket(new MarketId("se")));
        Assert.False(httpContextPermissionService.HasPermissionToMarket(new MarketId("no")));
    }

    private static HttpContextPermissionService CreateSut(IEnumerable<Claim> claims )
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        context.User.AddIdentity(new ClaimsIdentity(claims));
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

        return new HttpContextPermissionService(mockHttpContextAccessor.Object);
    }
}



