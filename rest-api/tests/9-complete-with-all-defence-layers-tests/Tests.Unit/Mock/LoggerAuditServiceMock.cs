using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Microsoft.Extensions.Logging;

namespace CompleteWithAllDefenceLayers.Tests.Unit.Mock;

public class LoggerAuditServiceMock : IAuditService
{
    private readonly ILogger<LoggerAuditService> logger;
    private readonly IPermissionService permissionService;

    public LoggerAuditServiceMock(ILogger<LoggerAuditService> logger, IPermissionService permissionService)
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

public class LoggerMock : ILogger<LoggerAuditService>
{
    public LoggerMock()
    {
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        //do nothing
    }
}