using System.Threading.Tasks;
using Defence.In.Depth.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Defence.In.Depth.Domain.Services;

public class LoggerAuditService : IAuditService
{
    private readonly ILogger<LoggerAuditService> logger;
    private readonly IPermissionService permissionService;

    public LoggerAuditService(ILogger<LoggerAuditService> logger, IPermissionService permissionService)
    {
        this.logger = logger;
        this.permissionService = permissionService;
    }

    public async Task Log(DomainEvent domainEvent, object payload)
    {
        var source = permissionService.UserId?.Value ?? permissionService.ClientId?.Value ?? "null";
        var amr = permissionService.AuthenticationMethods;

        logger.LogInformation("{Source} [{Amr}] {DomainEvent} {Payload}", source, amr, domainEvent, payload);

        await Task.CompletedTask;
    }
}