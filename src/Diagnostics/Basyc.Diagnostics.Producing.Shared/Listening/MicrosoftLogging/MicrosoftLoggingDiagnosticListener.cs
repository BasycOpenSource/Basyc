using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Producing.Shared.Listening.MicrosoftLogging;

public class MicrosoftLoggingDiagnosticListener : IDiagnosticListener
{
    public event EventHandler<LogEntry>? LogsReceived;
    public event EventHandler<ActivityEnd>? ActivityEndsReceived;
    public event EventHandler<ActivityStart>? ActivityStartsReceived;

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public void ReceiveLog(LogEntry logEntry)
    {
        LogsReceived?.Invoke(this, logEntry);
    }
}
