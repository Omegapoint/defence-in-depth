using System.Threading.Tasks;
using Defence.In.Depth.Infrastructure.Entities;

namespace Defence.In.Depth.Infrastructure;

public class ProductRepository : IProductRepository
{
    public async Task<ProductEntity> GetById(string id)
    {
        await Task.CompletedTask;
            
        // Please always use correct output encoding of input data "id" for
        // your query context. For example, parameterized SQL.
        // Here we have just hardcoded the id to the input.
        return new ProductEntity { Id = id, Name = "ProductSweden", MarketId = "se" };
    }
}