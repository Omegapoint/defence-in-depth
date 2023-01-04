using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;
using Moq;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductsServiceTests
{  
    [Fact]
    public async void GetById_ReturnsNoAccessToOperation_IfNoValidReadClaim()
    {
        var auditService = Mock.Of<IAuditService>();
        var productRepository = Mock.Of<IProductRepository>(MockBehavior.Strict);
        var permissionService = Mock.Of<IPermissionService>();
        
        var productService = new ProductService(permissionService, productRepository, auditService);

        var productId = new ProductId("se1");
        
        var (product, result) = await productService.GetById(productId);

        Assert.Equal(ReadDataResult.NoAccessToOperation, result);
        Assert.Null(product);
        
        Mock.Get(auditService).Verify(service => service.Log(DomainEvent.NoAccessToOperation, productId), Times.Once);
        Mock.Get(auditService).VerifyNoOtherCalls();
    }

    [Fact]
    public async void GetById_ReturnsNotFound_IfValidClaimButNotExisting()
    {
        var auditService = Mock.Of<IAuditService>(MockBehavior.Strict);
        var productRepository = Mock.Of<IProductRepository>();
        var permissionService = Mock.Of<IPermissionService>();

        Mock.Get(permissionService).SetupGet(service => service.CanReadProducts).Returns(true);

        var productService = new ProductService(permissionService, productRepository, auditService);
        
        var (product, result)  = await productService.GetById(new ProductId("notfound"));

        Assert.Equal(ReadDataResult.NotFound, result);
        Assert.Null(product);

        Mock.Get(auditService).VerifyNoOtherCalls();
    }

    [Fact]
    public async void GetById_ReturnsNoAccessToData_IfNotValidMarket()
    {
        var auditService = Mock.Of<IAuditService>();
        var productRepository = Mock.Of<IProductRepository>();
        var permissionService = Mock.Of<IPermissionService>();

        var productId = new ProductId("42");
        
        Mock.Get(productRepository)
            .Setup(repository => repository.GetById(productId))
            .ReturnsAsync(new Product(productId, new ProductName("My Name"), new MarketId("no")));

        Mock.Get(permissionService).SetupGet(service => service.CanReadProducts).Returns(true);
        Mock.Get(permissionService).SetupGet(service => service.MarketId).Returns(new MarketId("se"));
        
        var productService = new ProductService(permissionService, productRepository, auditService);

        var (product, result) = await productService.GetById(productId);

        Assert.Equal(ReadDataResult.NoAccessToData, result);
        Assert.Null(product);
        
        Mock.Get(auditService).Verify(service => service.Log(DomainEvent.NoAccessToData, productId), Times.Once);
        Mock.Get(auditService).VerifyNoOtherCalls();
    }

    // Testing successful resource access is important to verify that the
    // correct claim is needed to authorize access. If we did not, then
    // requiring a lower claim, e.g. "read:guest" would not be caught by the
    // NoValidReadClaim test above. This test will catch such configuration errors.
    [Fact]
    public async void GetById_ReturnsOk_IfValidClaims()
    {
        var auditService = Mock.Of<IAuditService>();
        var productRepository = Mock.Of<IProductRepository>();
        var permissionService = Mock.Of<IPermissionService>();

        var productId = new ProductId("42");
        
        Mock.Get(productRepository)
            .Setup(repository => repository.GetById(productId))
            .ReturnsAsync(new Product(productId, new ProductName("My Name"), new MarketId("se")));

        Mock.Get(permissionService).SetupGet(service => service.CanReadProducts).Returns(true);
        Mock.Get(permissionService).Setup(service => service.HasPermissionToMarket(new MarketId("se"))).Returns(true);
        
        var productService = new ProductService(permissionService, productRepository, auditService);

        var (product, result) = await productService.GetById(productId);

        Assert.Equal(ReadDataResult.Success, result);
        Assert.NotNull(product);
        
        Mock.Get(auditService).Verify(service => service.Log(DomainEvent.ProductRead, productId), Times.Once);
        Mock.Get(auditService).VerifyNoOtherCalls();
    }
}
