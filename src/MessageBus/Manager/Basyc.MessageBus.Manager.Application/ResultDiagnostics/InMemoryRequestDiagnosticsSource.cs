using Basyc.Diagnostics.Shared.Logging;
using System;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics
{
    public class InMemoryRequestDiagnosticsSource : IRequestDiagnosticsSource
    {
        public event EventHandler<LogsUpdatedArgs>? LogsReceived;
        public event EventHandler<ActivityStartsReceivedArgs>? ActivityStartsReceived;
        public event EventHandler<ActivityEndsReceivedArgs>? ActivityEndsReceived;

        private void OnLogsReceived(LogEntry[] logEntries)
        {
            LogsReceived?.Invoke(this, new LogsUpdatedArgs(logEntries));
        }

        private void OnActivityStartsReceived(ActivityStart[] activityStarts)
        {
            ActivityStartsReceived?.Invoke(this, new ActivityStartsReceivedArgs(activityStarts));
        }

        private void OnActivityEndsReceived(ActivityEnd[] activityEnds)
        {
            ActivityEndsReceived?.Invoke(this, new ActivityEndsReceivedArgs(activityEnds));
        }

        public void PushLog(LogEntry logEntry)
        {
            OnLogsReceived(new LogEntry[] { logEntry });
        }

        public void StartActivity(ActivityStart activityStart)
        {
            OnActivityStartsReceived(new ActivityStart[] { activityStart });
        }

        public void EndActivity(ActivityEnd activityEnd)
        {
            OnActivityEndsReceived(new ActivityEnd[] { activityEnd });
        }
    }
}