using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.DurationMap.Horizontal;

public static partial class LogAggregator
{
    public static ReadOnlyCollection<AggregatedLog> AggregateLogs(IEnumerable<LogEntry> logEntries, double pixelsPerMs, double logMinWidth, double logMaxWidth, double logWidthMultiplier)
    {
        //double boundingTimeDiffLimitMs = 4 / pixelsPerMs;
        double boundingTimeDiffLimitMs = 10 / pixelsPerMs;
        List<AggregatedLogInProgress> aggregatedLogsInProgress = new();
        foreach (var logEntry in logEntries)
        {
            var aggregatedLogInProgress = aggregatedLogsInProgress
            .FirstOrDefault(inprogress =>
            {
                var boundingTimeDiffMs = Math.Abs((inprogress.BoundingTime - logEntry.Time).TotalMilliseconds);
                return boundingTimeDiffMs <= boundingTimeDiffLimitMs;
            });
            var wasFound = aggregatedLogInProgress.Equals(default(AggregatedLogInProgress)) == false;
            if (wasFound is false)
            {
                aggregatedLogInProgress = new AggregatedLogInProgress(logEntry.Time, logEntry.LogLevel, logEntry.Service);
                aggregatedLogsInProgress.Add(aggregatedLogInProgress);
            }

            aggregatedLogInProgress.AddLog(logEntry);
        }

        var aggLogs = aggregatedLogsInProgress.Select(x => new AggregatedLog(x.BoundingTime, x.WorstLogLevel, x.Service, x.Logs)).ToList().AsReadOnly();
        return aggLogs;
    }

    private struct AggregatedLogInProgress
    {
        private readonly List<LogEntry> logs = new();

        public AggregatedLogInProgress(DateTimeOffset boundingTime, LogLevel worstLogLevel, ServiceIdentity service)
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
}
