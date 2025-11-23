using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Infrastructure.Entities;

namespace Defence.In.Depth.Infrastructure;

public class ProductRepository : IProductRepository
{
    public async Task<Product> GetById(ProductId productId)
    {
        await Task.CompletedTask;

        // We just create an entity, but normally this is a database query
        var entity = new ProductEntity { Id = productId.Value, Name = "Product in Sweden", MarketId = "se" };

        return Mapper.Map(entity);
    }
}