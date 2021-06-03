using Defence.In.Depth.Domain.Model;

namespace Defence.In.Depth.Domain.Services
{
    public interface IPermissionService
    {
        bool CanReadProducts { get; }

        bool CanWriteProducts { get; }
        
        MarketId MarketId { get; }
    }
}