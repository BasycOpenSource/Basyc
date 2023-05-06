using Basyc.Diagnostics.Receiving.Abstractions;
using Basyc.Diagnostics.Shared.Logging;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Infrastructure.Building;
using ActivityEndsReceivedArgs = Basyc.MessageBus.Manager.Application.ResultDiagnostics.ActivityEndsReceivedArgs;
using ActivityStartsReceivedArgs = Basyc.MessageBus.Manager.Application.ResultDiagnostics.ActivityStartsReceivedArgs;

namespace Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.Diagnostics;

public class BasycDiagnosticsReceiversRequestDiagnosticsSource : IRequestDiagnosticsSource
{
    private readonly IBasycDiagnosticsReceiverTraceIdMapper sessionMapper;

    public BasycDiagnosticsReceiversRequestDiagnosticsSource(IEnumerable<IDiagnosticReceiver> logReceivers,
        IBasycDiagnosticsReceiverTraceIdMapper sessionMapper)
    {
        foreach (var logReceiver in logReceivers)
        {
            logReceiver.LogsReceived += LogReceiver_LogsReceived;
            logReceiver.ActivityEndsReceived += LogReceiver_ActivityEndsReceived;
            logReceiver.ActivityStartsReceived += LogReceiver_ActivityStartsReceived;
        }

        this.sessionMapper = sessionMapper;
    }

    public event EventHandler<LogsUpdatedArgs>? LogsReceived;

    public event EventHandler<ActivityEndsReceivedArgs>? ActivityEndsReceived;

    public event EventHandler<ActivityStartsReceivedArgs>? ActivityStartsReceived;

    private void LogReceiver_LogsReceived(object? sender, LogsReceivedArgs e)
    {
        var mappedSessions = e.LogEntries.Select(x => new LogEntry(x.Service, sessionMapper.GetTraceId(x.TraceId), x.Time, x.LogLevel, x.Message, x.SpanId))
            .ToArray();
        LogsReceived?.Invoke(this, new LogsUpdatedArgs(mappedSessions));
    }

    private void LogReceiver_ActivityStartsReceived(object? sender, global::Basyc.Diagnostics.Receiving.Abstractions.ActivityStartsReceivedArgs e) => ActivityStartsReceived?.Invoke(this, new ActivityStartsReceivedArgs(e.ActivityStarts));

    private void LogReceiver_ActivityEndsReceived(object? sender, global::Basyc.Diagnostics.Receiving.Abstractions.ActivityEndsReceivedArgs e) => ActivityEndsReceived?.Invoke(this, new ActivityEndsReceivedArgs(e.ActivityEnds));
}
