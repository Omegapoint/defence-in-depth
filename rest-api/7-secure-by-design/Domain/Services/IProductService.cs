using Defence.In.Depth.Domain.Models;

namespace Defence.In.Depth.Domain.Services;

public interface IProductService
{
    Task<ServiceResult<Product>> GetById(ProductId productId);
}