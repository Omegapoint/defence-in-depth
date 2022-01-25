using Defence.In.Depth.Infrastructure.Entities;

namespace Defence.In.Depth.Infrastructure;

public interface IProductRepository
{
    Task<ProductEntity> GetById(string id);
}