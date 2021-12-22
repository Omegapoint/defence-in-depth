using System.Security.Claims;
using Defence.In.Depth.Domain.Model;
using Defence.In.Depth.Domain.Services;
using Xunit;
using System.Threading.Tasks;
using AutoMapper;
using Defence.In.Depth.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using Defence.In.Depth;

namespace CompleteWithAllDefenceLayers.Test;

[Trait("Category", "Unit")]
public class ProductsServiceTests
{  
    [Fact]
    public async void GetById_ReturnsNoAccessToOperation_IfNoValidReadClaim()
    {
        var claims = new[]
        {
                new Claim("not valid read claim", "true"),
                new Claim(ClaimSettings.UrnIdentityMarket, "se"),
        };
        var productService = CreateSUT(claims);
        
        var (product, result)  = await productService.GetById(new ProductId("productSE"));

        Assert.Equal(ReadDataResult.NoAccessToOperation, result);
        Assert.Null(product);
    }

    [Fact]
    public async void GetById_ReturnsNotFound_IfValidClaimButNotExisting()
    {
        var claims = new[]
        {
            new Claim("scope", ClaimSettings.ProductsRead),
            new Claim(ClaimSettings.UrnIdentityMarket, "se"),
        };
        var productService = CreateSUT(claims);
        
        var (product, result)  = await productService.GetById(new ProductId("notfound"));

        Assert.Equal(ReadDataResult.NotFound, result);
        Assert.Null(product);
    }

    [Fact]
    public async void GetById_ReturnsNoAccessToData_IfNotValidProduct()
    {
        var claims = new[]
        {
            new Claim("scope", ClaimSettings.ProductsRead),
            new Claim(ClaimSettings.UrnIdentityMarket, "se"),
        };
        var productService = CreateSUT(claims);
        
        var (product, result)  = await productService.GetById(new ProductId("productNO"));

        Assert.Equal(ReadDataResult.NoAccessToData, result);
        Assert.Null(product);
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
            new Claim("scope", ClaimSettings.ProductsRead),
            new Claim(ClaimSettings.UrnIdentityMarket, "se"),
        };
        var productService = CreateSUT(claims);
        
        var (product, result)  = await productService.GetById(new ProductId("productSE"));

        Assert.Equal(ReadDataResult.Success, result);
        Assert.NotNull(product);
    }

    private static IProductService CreateSUT(IEnumerable<Claim> claims )
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        context.User.AddIdentity(new ClaimsIdentity(claims));
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        
        var sut = new ProductService(
            new HttpContextPermissionService(mockHttpContextAccessor.Object), 
            new ProductRepository(), 
            new TestLoggingService(), 
            TestMapper.Create());
        
        return sut;
    }
}

public class TestLoggingService : IAuditService
{
    public async Task Log(DomainEvent domainEvent, object payload)
    {
        await Task.CompletedTask;
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


