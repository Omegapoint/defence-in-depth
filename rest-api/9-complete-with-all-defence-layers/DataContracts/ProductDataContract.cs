using System.Runtime.Serialization;

namespace Defence.In.Depth.DataContracts;

public interface IDataContract
{
}

[DataContract]
public record ProductDataContract(string? Id, string? Name) : IDataContract;