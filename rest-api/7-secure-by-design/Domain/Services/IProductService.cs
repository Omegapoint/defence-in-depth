using System.Threading.Tasks;
using Defence.In.Depth.Domain.Model;

namespace Defence.In.Depth.Domain.Services;

public interface IProductService
{
    Task<(Product? product, ReadDataResult result)> GetById(ProductId productId);
}