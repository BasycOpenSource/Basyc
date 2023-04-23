using Basyc.Diagnostics.Shared;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Basyc.Diagnostics.Producing.Shared.Listening.MicrosoftLogging;

internal class ExporterLogger : ILogger
{
    private readonly MicrosoftLoggingDiagnosticListener listener;

    public ExporterLogger(MicrosoftLoggingDiagnosticListener listener)
    {
        this.listener = listener;
    }

    public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (Activity.Current is null)
            return;

        string message = formatter.Invoke(state, exception);
        string traceId = Activity.Current.TraceId.ToString();
        string spanId = Activity.Current.SpanId.ToString();
        var logEntry = new Diagnostics.Shared.Logging.LogEntry(ServiceIdentity.ApplicationWideIdentity, traceId, DateTimeOffset.UtcNow, logLevel, message, spanId);
        listener.ReceiveLog(logEntry);
    }

    private class NullScope : IDisposable
    {
        public static IDisposable Instance { get; } = new NullScope();

        public void Dispose()
        {
        }
    }
}
