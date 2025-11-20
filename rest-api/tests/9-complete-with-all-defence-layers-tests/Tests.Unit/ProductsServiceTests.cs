using System.Security.Claims;
using CompleteWithAllDefenceLayers.Tests.Unit.Mock;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductsServiceTests
{  
    [Fact]
    public async Task GetById_ReturnsNoAccessToOperation_IfNoValidReadClaim()
    {
        var loggerMock = new LoggerMock();
        CreateSUT(out ProductRepository productRepository, out HttpContextPermissionService permissionService, out LoggerAuditService auditService, 
            [], loggerMock);

        var productService = new ProductService(permissionService, productRepository, auditService);

        var productId = new ProductId("se1");

        var (product, result) = await productService.GetById(productId);

        Assert.Equal(ReadDataResult.NoAccessToOperation, result);
        Assert.Null(product);
        Assert.Equal(1, loggerMock.CountNoAccessToOperation);
        Assert.Equal(1, loggerMock.TotalCount);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_IfValidClaimButNotExisting()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.Amr, ClaimSettings.AuthenticationMethodPassword),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsWrite)
        };

        var loggerMock = new LoggerMock();
        CreateSUT(out ProductRepository productRepository, out HttpContextPermissionService permissionService, out LoggerAuditService auditService, 
            claims, loggerMock);

        var productService = new ProductService(permissionService, productRepository, auditService);
        
        var (product, result)  = await productService.GetById(new ProductId("notfound"));

        Assert.Equal(ReadDataResult.NotFound, result);
        Assert.Null(product);
        Assert.Equal(0, loggerMock.TotalCount);
    }

    [Fact]
    public async Task GetById_ReturnsNoAccessToData_IfNotValidMarket()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.Amr, ClaimSettings.AuthenticationMethodPassword),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsWrite)
        };
        
        var loggerMock = new LoggerMock();
        CreateSUT(out ProductRepository productRepository, out HttpContextPermissionService permissionService, out LoggerAuditService auditService, 
            claims, loggerMock);
        
        var productId = new ProductId("no1");
        
        var productService = new ProductService(permissionService, productRepository, auditService);

        var (product, result) = await productService.GetById(productId);

        Assert.Equal(ReadDataResult.NoAccessToData, result);
        Assert.Null(product);
        Assert.Equal(1, loggerMock.CountNoAccessToData);
        Assert.Equal(1, loggerMock.TotalCount);
    }

    // Testing successful resource access is important to verify that the
    // correct claim is needed to authorize access. If we did not, then
    // requiring a lower claim, e.g. "read:guest" would not be caught by the
    // NoValidReadClaim test above. This test will catch such configuration errors.
    [Fact]
    public async Task GetById_ReturnsOk_IfValidClaims()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.Amr, ClaimSettings.AuthenticationMethodPassword),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsWrite)
        };

        var loggerMock = new LoggerMock();
        CreateSUT(out ProductRepository productRepository, out HttpContextPermissionService permissionService, out LoggerAuditService auditService, 
            claims, loggerMock);

        var productId = new ProductId("se1");

        var productService = new ProductService(permissionService, productRepository, auditService);

        var (product, result) = await productService.GetById(productId);

        Assert.Equal(ReadDataResult.Success, result);
        Assert.NotNull(product);
        Assert.Equal(1, loggerMock.CountProductRead);
        Assert.Equal(1, loggerMock.TotalCount);
    }

    private static void CreateSUT(out ProductRepository productRepository, out HttpContextPermissionService permissionService, out LoggerAuditService auditService, 
    IEnumerable<Claim> claims, LoggerMock loggerMock)
    {
        // Note that in a real-world application we usually need to mock repositories
        productRepository = new ProductRepository();
        var mockHttpContextAccessor = new HttpContextAccessorMock(new ClaimsIdentity(claims));
        permissionService = new HttpContextPermissionService(mockHttpContextAccessor);
        auditService = new LoggerAuditService(loggerMock, permissionService);
    }
}
