using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Infrastructure;

namespace Defence.In.Depth.Domain.Services;

public class ProductService : IProductService
{
    private readonly IPermissionService permissionService;
    private readonly IProductRepository productRepository;
    private readonly IAuditService auditService;

    public ProductService(
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

        await auditService.Log(DomainEvent.ProductRead, productId);

        return (product, ReadDataResult.Success);
    }
}