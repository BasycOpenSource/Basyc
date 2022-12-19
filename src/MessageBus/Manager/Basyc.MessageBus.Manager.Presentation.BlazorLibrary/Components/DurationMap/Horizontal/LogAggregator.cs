using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.DurationMap.Horizontal
{
	public static class LogAggregator
	{
		public struct AggregatedLog
		{
			public AggregatedLog(DateTimeOffset startTime, LogLevel worstLogLevel, ServiceIdentity service, IReadOnlyList<LogEntry> logs)
			{
				Time = startTime;
				WorstLogLevel = worstLogLevel;
				Service = service;
				Logs = logs;
			}

			public IReadOnlyList<LogEntry> Logs { get; init; }
			public DateTimeOffset Time { get; init; }
			public LogLevel WorstLogLevel { get; set; }
			public ServiceIdentity Service { get; init; }
		}



		private struct AggregatedLogInProgress
		{
			public AggregatedLogInProgress(DateTimeOffset boundingTime, LogLevel worstLogLevel, ServiceIdentity service)
			{
				BoundingTime = boundingTime;
				WorstLogLevel = worstLogLevel;
				Service = service;
			}
			private readonly List<LogEntry> logs { get; init; } = new();
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
		public static List<AggregatedLog> AggregateLogs(IEnumerable<LogEntry> logEntries, double pixelsPerMs, double logMinWidth, double logMaxWidth, double logWidthMultiplier)
		{
			double boundingTimeDiffLimitMs = 4 / pixelsPerMs;
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

			var aggLogs = aggregatedLogsInProgress.Select(x => new AggregatedLog(x.BoundingTime, x.WorstLogLevel, x.Service, x.Logs)).ToList();
			return aggLogs;
		}

	}
}
