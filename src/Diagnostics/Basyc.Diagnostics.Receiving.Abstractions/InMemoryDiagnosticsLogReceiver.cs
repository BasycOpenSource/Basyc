using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Receiving.Abstractions
{
	public class InMemoryDiagnosticsLogReceiver : IDiagnosticReceiver
	{
		public event EventHandler<LogsReceivedArgs>? LogsReceived;
		public event EventHandler<ActivityEndsReceivedArgs>? ActivityEndsReceived;
		public event EventHandler<ActivityStartsReceivedArgs>? ActivityStartsReceived;

		private void OnLogsReceived(LogEntry[] logEntries)
		{
			LogsReceived?.Invoke(this, new LogsReceivedArgs(logEntries));
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

		public Task StartReceiving()
		{
			return Task.CompletedTask;
		}
	}
}
