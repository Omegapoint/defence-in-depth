using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;

namespace CompleteWithAllDefenceLayers.Tests.Unit.Mock;

public class ProductServiceMock : IProductService
{
    private readonly IPermissionService permissionService;
    private readonly IProductRepository productRepository;
    private readonly IAuditService auditService;

    public ProductServiceMock(
        IPermissionService permissionService,
        IProductRepository productRepository,
        IAuditService auditService)
    {
        this.permissionService = permissionService;
        this.productRepository = productRepository;
        this.auditService = auditService;
    }

    public async Task<(Product? product, ReadDataResult result)> GetById(ProductId productId)
    {
        if (!permissionService.CanReadProducts)
        {
            await auditService.Log(DomainEvent.NoAccessToOperation, productId);

            return (null, ReadDataResult.NoAccessToOperation);
        }

        var product = await productRepository.GetById(productId);

        if (product == null)
        {
            return (null, ReadDataResult.NotFound);
        }

        if (!permissionService.HasPermissionToMarket(product.MarketId))
        {
            await auditService.Log(DomainEvent.NoAccessToData, productId);

            return (null, ReadDataResult.NoAccessToData);
        }
       
        // Here we can do more complex logic, like finding out if it is available in stores etc.

        await auditService.Log(DomainEvent.ProductRead, productId);

        return (product, ReadDataResult.Success);
    }
}