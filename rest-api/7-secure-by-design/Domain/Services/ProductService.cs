using System.Threading.Tasks;
using AutoMapper;
using Defence.In.Depth.Domain.Model;
using Defence.In.Depth.Infrastructure;

namespace Defence.In.Depth.Domain.Services
{
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

        public async Task<(Product product, ReadDataResult result)> GetById(ProductId productId)
        {
            if (!permissionService.CanReadProducts)
            {
                await auditService.Log(DomainEvent.NoAccessToOperation, productId);

                return (null, ReadDataResult.NoAccessToOperation);
            }

            var entity = await productRepository.GetById(productId.Value);

            if (entity == null)
            {
                return (null, ReadDataResult.NotFound);
            }

            var product = mapper.Map<Product>(entity);
            
            if (permissionService.MarketId != product.MarketId)
            {
                await auditService.Log(DomainEvent.NoAccessToData, productId);

                return (null, ReadDataResult.NoAccessToData);
            }

            await auditService.Log(DomainEvent.ProductRead, productId);

            return (product, ReadDataResult.Success);
        }
    }
}