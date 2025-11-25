using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Infrastructure.Entities;

namespace Defence.In.Depth.Infrastructure;

// A separate mapper class can be a strong pattern as the domain grows.
// It clarifies the relationship between the domain types and the rest.
// As classes grow larger, a mapping library like Mapperly can be very
// useful.
public static class Mapper
{
    public static Product Map(ProductEntity entity)
    {
        return new Product
        {
            Id = new ProductId(entity.Id),
            Name = new ProductName(entity.Name),
            MarketId = new MarketId(entity.MarketId)
        };
    }
}