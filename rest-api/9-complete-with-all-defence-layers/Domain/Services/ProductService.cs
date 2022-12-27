using AutoMapper;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Infrastructure;

namespace Defence.In.Depth.Domain.Services;

public class ProductService : IProductService
{
    private readonly IPermissionService permissionService;
    private readonly IProductRepository productRepository;
    private readonly IAuditService auditService;
    private readonly IMapper mapper;

    public ProductService(
        IPermissionService permissionService,
        IProductRepository productRepository,
        IAuditService auditService,
        IMapper mapper)
    {
        this.permissionService = permissionService;
        this.productRepository = productRepository;
        this.auditService = auditService;
        this.mapper = mapper;
    }

    // Note that in a real world domain, with more services and methods, it is important to
    // keep a clear pattern with access control first, before more complex business logic 
    // and data processing.
    // Verify access to operation should always be done first in any service method, but 
    // sometimes access to data need to be after intial data lookup or even after the 
    // business logic (e g for a search function).  
    public async Task<(Product? product, ReadDataResult result)> GetById(ProductId productId)
    {
        if (!permissionService.CanReadProducts)
        {
            await auditService.Log(DomainEvent.NoAccessToOperation, productId);

            return (null, ReadDataResult.NoAccessToOperation);
        }

        var entity = await productRepository.GetById(productId.Value);

        if (entity.Id == null)
        {
            return (null, ReadDataResult.NotFound);
        }

        var product = mapper.Map<Product>(entity);
            
        if (!permissionService.HasPermissionToMarket(product.MarketId))
        {
            await auditService.Log(DomainEvent.NoAccessToData, productId);

            return (null, ReadDataResult.NoAccessToData);
        }
       
        // When we have access to the specific product we can do more complex logic,
        // like finding out if it is availible in stores etc.

        await auditService.Log(DomainEvent.ProductRead, productId);

        return (product, ReadDataResult.Success);
    }
}