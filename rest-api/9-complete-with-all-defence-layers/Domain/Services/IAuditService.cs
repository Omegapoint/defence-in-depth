using System.Threading.Tasks;
using Defence.In.Depth.Domain.Model;

namespace Defence.In.Depth.Domain.Services
{
    public interface IAuditService
    {
        Task Log(DomainEvent domainEvent, object payload);
    }
}