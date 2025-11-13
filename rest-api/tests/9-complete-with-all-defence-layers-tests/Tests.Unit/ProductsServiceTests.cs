using System.Security.Claims;
using CompleteWithAllDefenceLayers.Tests.Unit.Mock;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductsServiceTests
{  
    [Fact]
    public async Task GetById_ReturnsNoAccessToOperation_IfNoValidReadClaim()
    {
        CreateSUT(out ProductRepositoryMock productRepository, out HttpContextPermissionService permissionService, out LoggerAuditServiceMock auditService, []);

        var productService = new ProductService(permissionService, productRepository, auditService);

        var productId = new ProductId("se1");

        var (product, result) = await productService.GetById(productId);

        Assert.Equal(ReadDataResult.NoAccessToOperation, result);
        Assert.Null(product);

        //TODO: Mock.Get(auditService).Verify(service => service.Log(DomainEvent.NoAccessToOperation, productId), Times.Once);
        //TODO: Mock.Get(auditService).VerifyNoOtherCalls();
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
        CreateSUT(out ProductRepositoryMock productRepository, out HttpContextPermissionService permissionService, out LoggerAuditServiceMock auditService, claims);

        var productService = new ProductService(permissionService, productRepository, auditService);
        
        var (product, result)  = await productService.GetById(new ProductId("notfound"));

        Assert.Equal(ReadDataResult.NotFound, result);
        Assert.Null(product);

        //TODO: Mock.Get(auditService).VerifyNoOtherCalls();
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
        CreateSUT(out ProductRepositoryMock productRepository, out HttpContextPermissionService permissionService, out LoggerAuditServiceMock auditService, claims);
        
        var productId = new ProductId("no1");
        
        var productService = new ProductService(permissionService, productRepository, auditService);

        var (product, result) = await productService.GetById(productId);

        Assert.Equal(ReadDataResult.NoAccessToData, result);
        Assert.Null(product);
        
        //TODO: Mock.Get(auditService).Verify(service => service.Log(DomainEvent.NoAccessToData, productId), Times.Once);
        //TODO: Mock.Get(auditService).VerifyNoOtherCalls();
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

        CreateSUT(out ProductRepositoryMock productRepository, out HttpContextPermissionService permissionService, out LoggerAuditServiceMock auditService, claims);

        var productId = new ProductId("se1");

        var productService = new ProductService(permissionService, productRepository, auditService);

        var (product, result) = await productService.GetById(productId);

        Assert.Equal(ReadDataResult.Success, result);
        Assert.NotNull(product);

        //TODO: Mock.Get(auditService).Verify(service => service.Log(DomainEvent.ProductRead, productId), Times.Once);
        //TODO: Mock.Get(auditService).VerifyNoOtherCalls();
    }

    private static void CreateSUT(out ProductRepositoryMock productRepository, out HttpContextPermissionService permissionService, out LoggerAuditServiceMock auditService, IEnumerable<Claim> claims)
    {
        productRepository = new ProductRepositoryMock();
        var mockHttpContextAccessor = new HttpContextAccessorMock(new ClaimsIdentity(claims));
        permissionService = new HttpContextPermissionService(mockHttpContextAccessor);
        auditService = new LoggerAuditServiceMock(new LoggerMock(), permissionService);
    }
}
