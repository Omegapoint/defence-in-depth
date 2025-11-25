using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Models;

namespace Defence.In.Depth.Endpoints;

// A separate mapper class can be a strong pattern as the domain grows.
// It clarifies the relationship between the domain types and the rest.
// As classes grow larger, a mapping library like Mapperly can be very
// useful.
public static class Mapper
{
    public static ProductDataContract Map(Product product)
    {
        return new ProductDataContract
        {
            Id = product.Id.Value,
            Name = product.Name.Value
        };
    }
}