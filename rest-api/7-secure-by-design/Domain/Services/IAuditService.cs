using Defence.In.Depth.Domain.Models;

namespace Defence.In.Depth.Domain.Services;

public interface IAuditService
{
    Task Log(DomainEvent domainEvent, object payload);
}