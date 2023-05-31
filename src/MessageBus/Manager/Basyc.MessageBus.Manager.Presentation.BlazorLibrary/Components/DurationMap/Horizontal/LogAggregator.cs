using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Logging;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.DurationMap.Horizontal;

public static partial class LogAggregator
{
    public static ReadOnlyCollection<AggregatedLog> AggregateLogs(ActivityContext activityContext, double pixelsPerMs, double logIconSizePx)
    {
        var logEntries = activityContext.Logs.ToArray();
        double boundingTimeDiffLimitMs = logIconSizePx * 1.5 / pixelsPerMs;
        var boundingTimeDiffLimitMsSpan = TimeSpan.FromMilliseconds(boundingTimeDiffLimitMs);
        List<AggregatedLogInProgressOld> aggregatedLogsInProgress = new();
        foreach (var logEntry in logEntries)
        {
            var aggregatedLogInProgress = aggregatedLogsInProgress
            .FirstOrDefault(inprogress =>
            {
                var boundingTimeDiffMs = Math.Abs((inprogress.BoundingTime - logEntry.Time).TotalMilliseconds);
                return boundingTimeDiffMs <= boundingTimeDiffLimitMs;
            });
            var wasFound = aggregatedLogInProgress.Equals(default(AggregatedLogInProgressOld)) == false;
            if (wasFound is false)
            {
                aggregatedLogInProgress = new AggregatedLogInProgressOld(logEntry.Time, logEntry.LogLevel, logEntry.Service);
                aggregatedLogsInProgress.Add(aggregatedLogInProgress);
            }

            aggregatedLogInProgress.AddLog(logEntry);
        }

        var aggLogs = aggregatedLogsInProgress.Select(x => new AggregatedLog(x.BoundingTime, x.BoundingTime + boundingTimeDiffLimitMsSpan, x.WorstLogLevel, x.Service, x.Logs)).ToList().AsReadOnly();
        return aggLogs;
    }

    //public static ReadOnlyCollection<AggregatedLog> AggregateLogs(ActivityContext activityContext, double pixelsPerMs, double logIconSizePx)
    //{
    //    var logEntries = activityContext.Logs.ToArray();
    //    if (activityContext.Logs.Count == 0)
    //        return new(Array.Empty<AggregatedLog>());

    //    var millisecondsPerCell = logIconSizePx / pixelsPerMs;
    //    var timeSpanPerCell = TimeSpan.FromMilliseconds(millisecondsPerCell);
    //    //var logCellCount = (int)Math.Floor(gridSizePx / logIconSizePx);
    //    //List<AggregatedLogInProgress?> cells = new();
    //    List<AggregatedLogInProgress> cells = new();

    //    var cellStartTime = activityContext.StartTime.AbsoluteTime;
    //    var cellEndTime = activityContext.StartTime.AbsoluteTime + timeSpanPerCell;
    //    var cellIndex = 0;
    //    cells.Add(new AggregatedLogInProgress(cellStartTime, cellEndTime, activityContext.Service));

    //    foreach (var logEntry in logEntries)
    //    {
    //        if (logEntry.Time < cellStartTime)
    //            throw new InvalidOperationException("It is expected that log entries are sorted by time");

    //        if (logEntry.Time > cellEndTime)
    //        {
    //            if ((logEntry.Time - cellEndTime).TotalMilliseconds >= millisecondsPerCell / 2)
    //            {
    //                while (logEntry.Time > cellEndTime)
    //                {
    //                    cellIndex++;
    //                    cellStartTime += timeSpanPerCell;
    //                    cellEndTime += timeSpanPerCell;
    //                    cells.Add(new AggregatedLogInProgress(cellStartTime, cellEndTime, activityContext.Service));
    //                }
    //            }
    //        }

    //        var cell = cells[cellIndex].Value();
    //        cell.Logs.Add(logEntry);
    //        if (cell.WorstLogLevel < logEntry.LogLevel && logEntry.LogLevel != LogLevel.None)
    //            cell.WorstLogLevel = logEntry.LogLevel;
    //    }

    //    return cells
    //        .Select(x => x.Value())
    //        .Select(x => new AggregatedLog(x.StartTime, x.EndTime, x.WorstLogLevel, x.Service, x.Logs))
    //        .ToList()
    //        .AsReadOnly();
    //}

    private struct AggregatedLogInProgressOld
    {
        private readonly List<LogEntry> logs = new();

        public AggregatedLogInProgressOld(DateTimeOffset boundingTime, LogLevel worstLogLevel, ServiceIdentity service)
        {
            BoundingTime = boundingTime;
            WorstLogLevel = worstLogLevel;
            Service = service;
        }

        public IReadOnlyList<LogEntry> Logs => logs;

        public DateTimeOffset BoundingTime { get; }

        public LogLevel WorstLogLevel { get; private set; }

        public ServiceIdentity Service { get; init; }

        public void AddLog(LogEntry logEntry)
        {
            if (logEntry.LogLevel > WorstLogLevel)
                WorstLogLevel = logEntry.LogLevel;
            logs.Add(logEntry);
        }
    }

    private struct AggregatedLogInProgress
    {
        public AggregatedLogInProgress(DateTimeOffset startTime, DateTimeOffset endTime, ServiceIdentity service)
        {
            StartTime = startTime;
            EndTime = endTime;
            Service = service;
        }

        public List<LogEntry> Logs { get; init; } = new List<LogEntry>();

        public DateTimeOffset StartTime { get; init; }

        public DateTimeOffset EndTime { get; init; }

        public LogLevel WorstLogLevel { get; set; }

        public ServiceIdentity Service { get; init; }
    }
}
