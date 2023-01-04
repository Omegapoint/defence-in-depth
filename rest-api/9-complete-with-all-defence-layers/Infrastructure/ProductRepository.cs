using AutoMapper;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Infrastructure.Entities;

namespace Defence.In.Depth.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly IMapper mapper;

    private readonly Dictionary<string, ProductEntity> data = new Dictionary<string, ProductEntity>{
        {"se1", new ProductEntity { Id = "se1", Name = "ProductSweden", MarketId = "se" }},
        {"no1", new ProductEntity { Id = "no1", Name = "ProductNorway", MarketId = "no" }}
    };

    public ProductRepository(IMapper mapper)
    {
        this.mapper = mapper;
    }

    public async Task<Product> GetById(ProductId productId)
    {
        await Task.CompletedTask;
            
        var entity = data.GetValueOrDefault(productId.Value);

        return mapper.Map<Product>(entity);
    }
}