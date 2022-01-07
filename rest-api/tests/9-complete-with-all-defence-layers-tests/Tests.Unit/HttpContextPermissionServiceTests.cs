using System.Collections.Generic;
using System.Security.Claims;
using Defence.In.Depth.Domain.Model;
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
                new Claim(ClaimSettings.AMR, ClaimSettings.AuthenticationMethodPassword),
                new Claim(ClaimSettings.UrnIdentityMarket, "se"),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsWrite)
        };
        var httpContextPermissionService = CreateSUT(claims);
        
        Assert.Equal(httpContextPermissionService.AuthenticationMethods, AuthenticationMethods.Password);
        Assert.Equal(httpContextPermissionService.CanReadProducts, true);
        Assert.Equal(httpContextPermissionService.CanWriteProducts, true);
        Assert.Equal(httpContextPermissionService.ClientId, new ClientId("client1"));
        Assert.Equal(httpContextPermissionService.MarketId, new MarketId("se"));
        Assert.Equal(httpContextPermissionService.UserId, new UserId("user1"));
    }

    [Fact]
    public void HasAllClaims_ButRead_DeniesRead()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.AMR, "pwd"),
                new Claim(ClaimSettings.UrnIdentityMarket, "se"),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsWrite)
        };
        var httpContextPermissionService = CreateSUT(claims);
        
        Assert.Equal(httpContextPermissionService.AuthenticationMethods, AuthenticationMethods.Password);
        Assert.Equal(httpContextPermissionService.CanReadProducts, false);
        Assert.Equal(httpContextPermissionService.CanWriteProducts, true);
        Assert.Equal(httpContextPermissionService.ClientId, new ClientId("client1"));
        Assert.Equal(httpContextPermissionService.MarketId, new MarketId("se"));
        Assert.Equal(httpContextPermissionService.UserId, new UserId("user1"));
    }

    [Fact]
    public void HasAllClaims_ButWrite_DeniesWrite()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.AMR, "pwd"),
                new Claim(ClaimSettings.UrnIdentityMarket, "se"),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead)
        };
        var httpContextPermissionService = CreateSUT(claims);
        
        Assert.Equal(httpContextPermissionService.AuthenticationMethods, AuthenticationMethods.Password);
        Assert.Equal(httpContextPermissionService.CanReadProducts, true);
        Assert.Equal(httpContextPermissionService.CanWriteProducts, false);
        Assert.Equal(httpContextPermissionService.ClientId, new ClientId("client1"));
        Assert.Equal(httpContextPermissionService.MarketId, new MarketId("se"));
        Assert.Equal(httpContextPermissionService.UserId, new UserId("user1"));
    }

    private static HttpContextPermissionService CreateSUT(IEnumerable<Claim> claims )
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        context.User.AddIdentity(new ClaimsIdentity(claims));
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        
        var sut = new HttpContextPermissionService(mockHttpContextAccessor.Object);
        
        return sut;
    }
}



