using System.Diagnostics.CodeAnalysis;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public class RequestDiagnosticsRepository : IRequestDiagnosticsRepository
{
	private readonly Dictionary<string, RequestDiagnostic> traceIdToContextMap = new();

	public RequestDiagnosticsRepository(IEnumerable<IRequestDiagnosticsSource> logSources)
	{
		foreach (var logSource in logSources)
		{
			logSource.LogsReceived += LogSource_LogsReceived;
			logSource.ActivityStartsReceived += LogSource_ActivityStartsReceived;
			logSource.ActivityEndsReceived += LogSource_ActivityEndsReceived;
		}
	}

	public RequestDiagnostic CreateDiagnostics(string traceId)
	{
		var loggingContext = new RequestDiagnostic(traceId);
		traceIdToContextMap.Add(traceId, loggingContext);
		return loggingContext;
	}

	public bool TryGetDiagnostics(string traceId, [NotNullWhen(true)] out RequestDiagnostic? diagnosticContext)
	{
		return traceIdToContextMap.TryGetValue(traceId, out diagnosticContext);
	}

	private void LogSource_ActivityStartsReceived(object? sender, ActivityStartsReceivedArgs e)
	{
		foreach (var activityStart in e.ActivityStarts)
		{
			if (TryGetDiagnostics(activityStart.TraceId, out var loggingContext) is false)
				return;

			loggingContext.AddStartActivity(activityStart);
		}
	}

	private void LogSource_ActivityEndsReceived(object? sender, ActivityEndsReceivedArgs e)
	{
		foreach (var activityEnd in e.ActivityEnds)
		{
			if (TryGetDiagnostics(activityEnd.TraceId, out var loggingContext) is false)
				return;

			loggingContext.AddEndActivity(activityEnd);
		}
	}

	private void LogSource_LogsReceived(object? sender, LogsUpdatedArgs e)
	{
		foreach (var logEntry in e.NewLogEntries)
		{
			if (TryGetDiagnostics(logEntry.TraceId, out var loggingContext) is false)
				return;
			loggingContext.AddLog(logEntry);
		}
	}
}
