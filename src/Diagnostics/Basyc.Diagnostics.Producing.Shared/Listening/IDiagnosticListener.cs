using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Producing.Shared.Listening
{
    public interface IDiagnosticListener
    {
        Task Start();
        event EventHandler<LogEntry> LogsReceived;
        event EventHandler<ActivityEnd> ActivityEndsReceived;
        event EventHandler<ActivityStart> ActivityStartsReceived;
    }
}