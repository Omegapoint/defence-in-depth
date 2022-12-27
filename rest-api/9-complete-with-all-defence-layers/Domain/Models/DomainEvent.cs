namespace Defence.In.Depth.Domain.Models;

public enum DomainEvent
{
    None = 0,

    NoAccessToOperation = 1,

    NoAccessToData = 2,

    ProductRead = 3
}