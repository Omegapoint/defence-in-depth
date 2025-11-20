using Defence.In.Depth.Domain.Services;
using Microsoft.Extensions.Logging;

namespace CompleteWithAllDefenceLayers.Tests.Unit.Mock;

public class LoggerMock : ILogger<LoggerAuditService>
{
    public int TotalCount {get;set;}
    public int CountNoAccessToOperation {get;set;}
    public int CountNoAccessToData {get;set;}
    public int CountProductRead {get;set;}

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
        TotalCount++;

        var exceptionMsg = state == null ? string.Empty : state.ToString();
        if(exceptionMsg == null)
        {
            exceptionMsg = "";
        }

        if (exceptionMsg.Contains("NoAccessToOperation"))
        {
            CountNoAccessToOperation++;
        }

        if (exceptionMsg.Contains("NoAccessToData"))
        {
            CountNoAccessToData++;
        }

        if (exceptionMsg.Contains("ProductRead"))
        {
            CountProductRead++;
        }
    }
}