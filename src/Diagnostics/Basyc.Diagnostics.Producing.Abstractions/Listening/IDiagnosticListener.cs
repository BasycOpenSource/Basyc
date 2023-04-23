using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Producing.Shared.Listening;

public interface IDiagnosticListener
{
    event EventHandler<LogEntry> LogsReceived;

    event EventHandler<ActivityEnd> ActivityEndsReceived;

    event EventHandler<ActivityStart> ActivityStartsReceived;

    Task Start();
}
