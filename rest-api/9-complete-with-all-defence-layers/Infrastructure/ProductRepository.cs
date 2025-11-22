using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Infrastructure.Entities;

namespace Defence.In.Depth.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly Dictionary<string, ProductEntity> data = new()
    {
        { "se1", new ProductEntity { Id = "se1", Name = "ProductSweden", MarketId = "se" } },
        { "no1", new ProductEntity { Id = "no1", Name = "ProductNorway", MarketId = "no" } }
    };

    public async Task<Product?> GetById(ProductId productId)
    {
        await Task.CompletedTask;

        var entity = data.GetValueOrDefault(productId.Value);

        if (entity == null)
        {
            return null;
        }

        return new Product(new ProductId(entity.Id ?? "empty"), new ProductName(entity.Name ?? "empty"), new MarketId(entity.MarketId ?? "empty"));
    }
}