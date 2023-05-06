using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Producing.Shared.Listening;

#pragma warning disable CA1003 // Use generic event handler instances

public interface IDiagnosticListener
{
    event EventHandler<LogEntry> LogsReceived;

    event EventHandler<ActivityEnd> ActivityEndsReceived;

    event EventHandler<ActivityStart> ActivityStartsReceived;

    Task Start();
}
