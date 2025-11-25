using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Infrastructure;

namespace Defence.In.Depth.Domain.Services;

public class ProductService(
    IPermissionService permissionService,
    IProductRepository productRepository,
    IAuditService auditService)
    : IProductService
{
    // Note that in a real world domain, with more services and methods, it is important to
    // keep a clear pattern with access control and input validation as early as possible, 
    // before any business logic and data processing.
    // Verify access to operation should always be done first in any service method, but 
    // sometimes access to data need to be after intial data lookup or even after the 
    // business logic (e g for a search function).  
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
       
        // Here we can do more complex logic, like finding out if it is available in stores etc.

        await auditService.Log(DomainEvent.ProductRead, productId);

        return ServiceResult<Product>.Success(product);
    }
}