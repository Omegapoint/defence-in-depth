using Defence.In.Depth.Domain.Models;

namespace Defence.In.Depth.Domain.Services;

public interface IProductService
{
    Task<(Product? product, ReadDataResult result)> GetById(ProductId productId);
}