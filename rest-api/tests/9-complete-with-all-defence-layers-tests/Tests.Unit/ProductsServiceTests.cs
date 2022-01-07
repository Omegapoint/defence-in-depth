using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Defence.In.Depth;
using Defence.In.Depth.Domain.Model;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductsServiceTests
{  
    [Fact]
    public async void GetById_ReturnsNoAccessToOperation_IfNoValidReadClaim()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Scope, "not valid read claim"),
                new Claim(ClaimSettings.UrnIdentityMarket, "se"),
        };
        var mockAuditService = new Mock<IAuditService>();
        var productService = CreateSUT(claims, mockAuditService.Object);
        
        var (product, result)  = await productService.GetById(new ProductId("productSE"));

        Assert.Equal(ReadDataResult.NoAccessToOperation, result);
        Assert.Null(product);
        mockAuditService.Verify(_ => _.Log(It.IsAny<DomainEvent>(), It.IsAny<object>()), Times.Once);
        mockAuditService.Verify(_ => _.Log(DomainEvent.NoAccessToOperation, It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async void GetById_ReturnsNotFound_IfValidClaimButNotExisting()
    {
        var claims = new[]
        {
            new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead),
            new Claim(ClaimSettings.UrnIdentityMarket, "se"),
        };
        var mockAuditService = new Mock<IAuditService>();
        var productService = CreateSUT(claims, mockAuditService.Object);
        
        var (product, result)  = await productService.GetById(new ProductId("notfound"));

        Assert.Equal(ReadDataResult.NotFound, result);
        Assert.Null(product);
        mockAuditService.Verify(_ => _.Log(It.IsAny<DomainEvent>(), It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async void GetById_ReturnsNoAccessToData_IfNotValidProduct()
    {
        var claims = new[]
        {
            new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead),
            new Claim(ClaimSettings.UrnIdentityMarket, "se"),
        };
        var mockAuditService = new Mock<IAuditService>();
        var productService = CreateSUT(claims, mockAuditService.Object);
        
        var (product, result)  = await productService.GetById(new ProductId("productNO"));

        Assert.Equal(ReadDataResult.NoAccessToData, result);
        Assert.Null(product);
        mockAuditService.Verify(_ => _.Log(It.IsAny<DomainEvent>(), It.IsAny<object>()), Times.Once);
        mockAuditService.Verify(_ => _.Log(DomainEvent.NoAccessToData, It.IsAny<object>()), Times.Once);
    }

    // Testing successful resource access is important to verify that the
    // correct claim is needed to authorize access.  If we did not, then
    // requiring a lower claim, e.g. "read:guest" would not be caught by the
    // NoValidScopeClaim test above; This test will catch such configuration errors.
    [Fact]
    public async void GetById_ReturnsOk_IfValidClaims()
    {
        var claims = new[]
        {
            new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead),
            new Claim(ClaimSettings.UrnIdentityMarket, "se"),
        };
        var mockAuditService = new Mock<IAuditService>();
        var productService = CreateSUT(claims, mockAuditService.Object);
        
        var (product, result)  = await productService.GetById(new ProductId("productSE"));

        Assert.Equal(ReadDataResult.Success, result);
        Assert.NotNull(product);
        mockAuditService.Verify(_ => _.Log(It.IsAny<DomainEvent>(), It.IsAny<object>()), Times.Once);
        mockAuditService.Verify(_ => _.Log(DomainEvent.ProductRead, It.IsAny<object>()), Times.Once);
    }

    private static IProductService CreateSUT(IEnumerable<Claim> claims, IAuditService? auditService = null)
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        context.User.AddIdentity(new ClaimsIdentity(claims));
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        
        var sut = new ProductService(
            new HttpContextPermissionService(mockHttpContextAccessor.Object), 
            new ProductRepository(), 
            auditService ?? new Mock<IAuditService>().Object, 
            TestMapper.Create());
        
        return sut;
    }
}

public static class TestMapper
{
    public static IMapper Create()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });

        return mappingConfig.CreateMapper();
    }
}


