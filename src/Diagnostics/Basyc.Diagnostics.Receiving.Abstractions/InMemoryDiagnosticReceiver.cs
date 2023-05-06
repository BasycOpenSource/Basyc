using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Receiving.Abstractions;

public class InMemoryDiagnosticReceiver : IDiagnosticReceiver
{
    public event EventHandler<LogsReceivedArgs>? LogsReceived;

    public event EventHandler<ActivityEndsReceivedArgs>? ActivityEndsReceived;

    public event EventHandler<ActivityStartsReceivedArgs>? ActivityStartsReceived;

    public Task StartReceiving() => Task.CompletedTask;

    public void PushLog(LogEntry logEntry) => OnLogsReceived(new[] { logEntry });

    public void StartActivity(ActivityStart activityStart) => OnActivityStartsReceived(new[] { activityStart });

    public void EndActivity(ActivityEnd activityEnd) => OnActivityEndsReceived(new[] { activityEnd });

    private void OnLogsReceived(LogEntry[] logEntries) => LogsReceived?.Invoke(this, new(logEntries));

    private void OnActivityStartsReceived(ActivityStart[] activityStarts) => ActivityStartsReceived?.Invoke(this, new(activityStarts));

    private void OnActivityEndsReceived(ActivityEnd[] activityEnds) => ActivityEndsReceived?.Invoke(this, new(activityEnds));
}
