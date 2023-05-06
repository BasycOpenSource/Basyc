using Basyc.Diagnostics.Providers.SignalR.Shared.DTOs;
using Basyc.Diagnostics.Receiving.Abstractions;
using Basyc.Diagnostics.Shared.Logging;
using Basyc.Diagnostics.SignalR.Shared;
using Basyc.Diagnostics.SignalR.Shared.DTOs;
using Basyc.Extensions.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace Basyc.Diagnostics.Receiving.SignalR;

public class SignalRDiagnosticsLogReceiver : IDiagnosticReceiver, IReceiversMethodsServerCanCall
{
    private readonly IStrongTypedHubConnectionPusherAndReceiver<IServerMethodsReceiversCanCall, IReceiversMethodsServerCanCall> hubConnection;

    public SignalRDiagnosticsLogReceiver(IOptions<SignalRLogReceiverOptions> options)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(options.Value.SignalRServerReceiverHubUri!)
            .WithAutomaticReconnect()
            .BuildStrongTyped<IServerMethodsReceiversCanCall, IReceiversMethodsServerCanCall>(this);
    }

    public event EventHandler<LogsReceivedArgs>? LogsReceived;

    public event EventHandler<ActivityEndsReceivedArgs>? ActivityEndsReceived;

    public event EventHandler<ActivityStartsReceivedArgs>? ActivityStartsReceived;

    public async Task StartReceiving() => await hubConnection.StartAsync();

    public Task ReceiveChangesFromServer(ChangesSignalRDto changes)
    {
        if (changes.Logs.Any())
            ReceiveLogEntriesFromServer(changes.Logs);

        if (changes.ActivityStarts.Any())
            ReceiveStartedActivitiesFromServer(changes.ActivityStarts);

        if (changes.ActivityEnds.Any())
            ReceiveEndedActivitiesFromServer(changes.ActivityEnds);

        return Task.CompletedTask;
    }

    private void OnLogsReceived(LogEntry[] logEntries) => LogsReceived?.Invoke(this, new(logEntries));

    private void OnActivityEndsReceived(ActivityEnd[] activities) => ActivityEndsReceived?.Invoke(this, new(activities));

    private void OnActivityStartsReceived(ActivityStart[] activityStarts) => ActivityStartsReceived?.Invoke(this, new(activityStarts));

    private void ReceiveLogEntriesFromServer(LogEntrySignalRDto[] logEntriesDtos)
    {
        var logEntries = logEntriesDtos
            .Select(LogEntrySignalRDto.ToEntry)
            .ToArray();
        OnLogsReceived(logEntries);
    }

    private void ReceiveStartedActivitiesFromServer(ActivityStartSignalRDto[] activitiesDtos)
    {
        var activities = activitiesDtos
            .Select(ActivityStartSignalRDto.ToEntry)
            .ToArray();

        OnActivityStartsReceived(activities);
    }

    private void ReceiveEndedActivitiesFromServer(ActivityEndSignalRDto[] activitiesDtos)
    {
        var activities = activitiesDtos
            .Select(ActivityEndSignalRDto.ToEntry)
            .ToArray();

        OnActivityEndsReceived(activities);
    }
}
