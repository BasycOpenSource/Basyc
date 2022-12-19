using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics
{
    public class RequestDiagnosticsManager : IRequestDiagnosticsManager
    {
        private readonly Dictionary<string, RequestDiagnosticContext> traceIdToContextMap = new();

        public RequestDiagnosticsManager(IEnumerable<IRequestDiagnosticsSource> logSources)
        {
            foreach (var logSource in logSources)
            {
                logSource.LogsReceived += LogSource_LogsReceived;
                logSource.ActivityStartsReceived += LogSource_ActivityStartsReceived;
                logSource.ActivityEndsReceived += LogSource_ActivityEndsReceived;
            }
        }

        private void LogSource_ActivityStartsReceived(object? sender, ActivityStartsReceivedArgs e)
        {
            foreach (var activityStart in e.ActivityStarts)
            {
                if (TryGetDiagnostics(activityStart.TraceId, out var loggingContext) is false)
                    return;

                loggingContext.StartActivity(activityStart);
            }
        }

        private void LogSource_ActivityEndsReceived(object? sender, ActivityEndsReceivedArgs e)
        {
            foreach (var activityEnd in e.ActivityEnds)
            {
                if (TryGetDiagnostics(activityEnd.TraceId, out var loggingContext) is false)
                    return;

                loggingContext.EndActivity(activityEnd);
            }
        }

        private void LogSource_LogsReceived(object? sender, LogsUpdatedArgs e)
        {
            foreach (var logEntry in e.NewLogEntries)
            {
                if (TryGetDiagnostics(logEntry.TraceId, out var loggingContext) is false)
                    return;
                loggingContext.Log(logEntry);
            }
        }

        public RequestDiagnosticContext CreateDiagnostics(string traceId)
        {
            RequestDiagnosticContext loggingContext = new RequestDiagnosticContext(traceId);
            traceIdToContextMap.Add(traceId, loggingContext);
            return loggingContext;
        }

        public bool TryGetDiagnostics(string traceId, [NotNullWhen(true)] out RequestDiagnosticContext? diagnosticContext)
        {
            return traceIdToContextMap.TryGetValue(traceId, out diagnosticContext);
        }
    }
}