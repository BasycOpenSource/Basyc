using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Producing.Abstractions;

/// <summary>
///     Dummy producer implementation that does nothing.
/// </summary>
public class NullDiagnosticsExporter : IDiagnosticsExporter
{
    public Task EndActivity(ActivityEnd activityEnd) => Task.CompletedTask;

    public Task ProduceLog(LogEntry logEntry) => Task.CompletedTask;

    public Task StartActivity(ActivityStart activityStart) => Task.CompletedTask;

    public Task<bool> StartAsync() => Task.FromResult(true);
}
