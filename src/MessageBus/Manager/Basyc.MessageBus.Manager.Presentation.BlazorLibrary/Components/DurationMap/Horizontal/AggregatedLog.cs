using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.DurationMap.Horizontal;

public struct AggregatedLog
{
    public AggregatedLog(DateTimeOffset startTime, DateTimeOffset endTime, LogLevel worstLogLevel, ServiceIdentity service, IReadOnlyList<LogEntry> logs)
    {
        StartTime = startTime;
        EndTime = endTime;
        WorstLogLevel = worstLogLevel;
        Service = service;
        Logs = logs;
    }

    public IReadOnlyList<LogEntry> Logs { get; init; }

    public DateTimeOffset StartTime { get; init; }

    public DateTimeOffset EndTime { get; }

    public LogLevel WorstLogLevel { get; set; }

    public ServiceIdentity Service { get; init; }
}
