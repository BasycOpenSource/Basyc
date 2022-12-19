using Basyc.Diagnostics.Receiving.Abstractions;
using Basyc.Diagnostics.Shared.Logging;
using Basyc.Diagnostics.SignalR.Shared;
using Basyc.Diagnostics.SignalR.Shared.DTOs;
using Basyc.Extensions.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace Basyc.Diagnostics.Receiving.SignalR
{
	public class SignalRDiagnosticsLogReceiver : IDiagnosticReceiver, IReceiversMethodsServerCanCall
	{
		private readonly IStrongTypedHubConnectionPusherAndReceiver<IServerMethodsReceiversCanCall, IReceiversMethodsServerCanCall> hubConnection;
		public event EventHandler<LogsReceivedArgs>? LogsReceived;
		public event EventHandler<ActivityEndsReceivedArgs>? ActivityEndsReceived;
		public event EventHandler<ActivityStartsReceivedArgs>? ActivityStartsReceived;

		public SignalRDiagnosticsLogReceiver(IOptions<SignalRLogReceiverOptions> options)
		{
			hubConnection = new HubConnectionBuilder()
				.WithUrl(options.Value.SignalRServerReceiverHubUri!)
				.WithAutomaticReconnect()
				.BuildStrongTyped<IServerMethodsReceiversCanCall, IReceiversMethodsServerCanCall>(this);
		}

		private void OnLogsReceived(LogEntry[] logEntries)
		{
			LogsReceived?.Invoke(this, new LogsReceivedArgs(logEntries));
		}

		private void OnActivityEndsReceived(ActivityEnd[] activities)
		{
			ActivityEndsReceived?.Invoke(this, new ActivityEndsReceivedArgs(activities));
		}

		private void OnActivityStartsReceived(ActivityStart[] activityStarts)
		{
			ActivityStartsReceived?.Invoke(this, new ActivityStartsReceivedArgs(activityStarts));
		}

		public async Task StartReceiving()
		{
			await hubConnection.StartAsync();
		}

		public Task ReceiveChangesFromServer(ChangesSignalRDTO changes)
		{
			if (changes.Logs.Any())
				receiveLogEntriesFromServer(changes.Logs);

			if (changes.ActivityStarts.Any())
				receivStartedActivitiesFromServer(changes.ActivityStarts);

			if (changes.ActivityEnds.Any())
				receiveEndedActivitiesFromServer(changes.ActivityEnds);

			return Task.CompletedTask;
		}


		private void receiveLogEntriesFromServer(LogEntrySignalRDTO[] logEntriesDTOs)
		{
			var logEntries = logEntriesDTOs
				.Select(x => LogEntrySignalRDTO.ToEntry(x))
				.ToArray();
			OnLogsReceived(logEntries);
		}

		private void receivStartedActivitiesFromServer(ActivityStartSignalRDTO[] activitiesDTOs)
		{
			var activities = activitiesDTOs
				.Select(x => ActivityStartSignalRDTO.ToEntry(x))
				.ToArray();

			OnActivityStartsReceived(activities);
		}

		private void receiveEndedActivitiesFromServer(ActivityEndSignalRDTO[] activitiesDTOs)
		{
			var activities = activitiesDTOs
				.Select(x => ActivityEndSignalRDTO.ToEntry(x))
				.ToArray();

			OnActivityEndsReceived(activities);
		}
	}
}
