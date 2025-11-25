namespace Defence.In.Depth.Infrastructure.Entities;

public record ProductEntity
{
    public required string Id { get; init; }

    public required string Name { get; init; }
        
    public required string MarketId { get; init; }
}