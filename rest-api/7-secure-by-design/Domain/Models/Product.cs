namespace Defence.In.Depth.Domain.Models;

public class Product
{
    public Product(ProductId id, ProductName name, MarketId marketId)
    {
        Id = id;
        Name = name;
        MarketId = marketId;
    }

    public ProductId Id { get; }

    public ProductName Name { get; }

    public MarketId MarketId { get; }
}