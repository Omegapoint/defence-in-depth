using System.Collections.Generic;
using System.Threading.Tasks;
using Defence.In.Depth.Infrastructure.Entities;

namespace Defence.In.Depth.Infrastructure;

public class ProductRepository : IProductRepository
{
    private Dictionary<string, ProductEntity> _repo = new Dictionary<string, ProductEntity>{
        {"productSE", new ProductEntity { Id = "productSE", Name = "product", MarketId = "SE" }},
        {"productNO", new ProductEntity { Id = "productNO", Name = "product", MarketId = "NO" }}
    };
    public async Task<ProductEntity> GetById(string id)
    {
        await Task.CompletedTask;
            
        // Please always use correct output encoding of input data "id" for
        // your query context.  For example, parameterized SQL.
        if(!_repo.ContainsKey(id))
            return null;
        return _repo[id];
    }
}