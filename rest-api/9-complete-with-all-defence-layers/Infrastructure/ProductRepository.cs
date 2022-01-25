using Defence.In.Depth.Infrastructure.Entities;

namespace Defence.In.Depth.Infrastructure;

public class ProductRepository : IProductRepository
{
    private Dictionary<string, ProductEntity> _repo = new Dictionary<string, ProductEntity>{
        {"se1", new ProductEntity { Id = "se1", Name = "ProductSweden", MarketId = "se" }},
        {"no1", new ProductEntity { Id = "no1", Name = "ProductNorway", MarketId = "no" }}
    };

    public async Task<ProductEntity> GetById(string id)
    {
        await Task.CompletedTask;
            
        // Please always use correct output encoding of input data "id" for
        // your query context.  For example, parameterized SQL.
        if(!_repo.ContainsKey(id))
        {
            return new ProductEntity();
        }

        return _repo[id];
    }
}