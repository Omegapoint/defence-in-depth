namespace Defence.In.Depth.Domain.Models;

public class Product
{
    public required ProductId Id { get; init; }

    public required ProductName Name { get; init; }

    public required MarketId MarketId { get; init; }
}