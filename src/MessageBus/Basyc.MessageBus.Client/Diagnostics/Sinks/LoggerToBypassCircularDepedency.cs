using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Client.Diagnostics.Sinks;

/// <summary>
///     Use this class instead of <see cref="ILogger{TCategoryName}" />
///     when having problem injection <see cref="ILogger{TCategoryName}" /> with error indicating circular depedency.
/// </summary>
public class LoggerToBypassCircularDepedency<TCategory> : ILogger<TCategory>
{
    private readonly ILogger<TCategory> logger;

    public LoggerToBypassCircularDepedency(ILoggerFactory factory)
    {
        logger = new Logger<TCategory>(factory);
    }

    public IDisposable BeginScope<TState>(TState state) => logger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) => logger.Log(logLevel, eventId, state, exception, formatter);
}
