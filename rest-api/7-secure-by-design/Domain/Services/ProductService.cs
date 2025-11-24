using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Infrastructure;

namespace Defence.In.Depth.Domain.Services;

public class ProductService(
    IPermissionService permissionService,
    IProductRepository productRepository,
    IAuditService auditService)
    : IProductService
{
    public async Task<ServiceResult<Product>> GetById(ProductId productId)
    {
        if (!permissionService.CanReadProducts)
        {
            await auditService.Log(DomainEvent.NoAccessToOperation, productId);

            return ServiceResult<Product>.NoAccessToOperation;
        }

        var product = await productRepository.GetById(productId);

        if (product == null)
        {
            return ServiceResult<Product>.NotFound;
        }

        if (!permissionService.HasPermissionToMarket(product.MarketId))
        {
            await auditService.Log(DomainEvent.NoAccessToData, productId);

            return ServiceResult<Product>.NoAccessToData;
        }

        await auditService.Log(DomainEvent.ProductRead, productId);

        return ServiceResult<Product>.Success(product);
    }
}