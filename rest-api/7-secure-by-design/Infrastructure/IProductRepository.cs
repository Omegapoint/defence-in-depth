using Defence.In.Depth.Domain.Models;

namespace Defence.In.Depth.Infrastructure;

public interface IProductRepository
{
    Task<Product> GetById(ProductId productId);
}