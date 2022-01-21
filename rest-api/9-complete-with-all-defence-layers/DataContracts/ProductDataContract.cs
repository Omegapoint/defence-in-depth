using System.Runtime.Serialization;

namespace Defence.In.Depth.DataContracts;

[DataContract]
public record ProductDataContract
{
    [DataMember]
    public string? Id { get; init; }
        
    [DataMember]
    public string? Name { get; init; }
}