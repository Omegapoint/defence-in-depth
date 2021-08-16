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
        private readonly IMapper mapper;

        public ProductService(IPermissionService permissionService, IProductRepository productRepository, IMapper mapper)
        {
            this.permissionService = permissionService;
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        public async Task<(Product product, ReadDataResult result)> GetById(ProductId productId)
        {
            if (!permissionService.CanReadProducts)
            {
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
                return (null, ReadDataResult.NoAccessToData);
            }

            return (product, ReadDataResult.Success);
        }
    }
}