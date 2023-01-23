using System.Runtime.Serialization;

namespace Defence.In.Depth.DataContracts;

public interface IDataContract
{
}

public record ProductDataContract(string? Id, string? Name) : IDataContract;