using System.Runtime.Serialization;

namespace Defence.In.Depth.DataContracts;

public interface IDataContract;

public record ProductDataContract : IDataContract
{
    public required string Id { get; init; }

    public required string Name { get; init; }
}