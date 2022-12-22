using Basyc.Diagnostics.Producing.Shared;
using Basyc.Diagnostics.Receiving.SignalR;
using Basyc.Diagnostics.Shared.Logging;
using Basyc.Diagnostics.SignalR.Shared;
using Basyc.Diagnostics.SignalR.Shared.DTOs;
using Basyc.Extensions.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Basyc.Diagnostics.Producing.SignalR;

public class SignalRDiagnosticsExporter : IDiagnosticsExporter
{
    private readonly Channel<ChangesSignalRDTO> signalRChannel = Channel.CreateUnbounded<ChangesSignalRDTO>(new UnboundedChannelOptions() { SingleReader = true });

    private readonly IStrongTypedHubConnectionPusher<IServerMethodsProducersCanCall> hubConnection;

    public SignalRDiagnosticsExporter(IOptions<SignalRLogReceiverOptions> options)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(options.Value.SignalRServerUri!)
            .WithAutomaticReconnect()
            .BuildStrongTyped<IServerMethodsProducersCanCall>();
    }

    /// <summary>
    /// Returns false when failed to connect
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StartAsync()
    {
        await hubConnection.StartAsync();
        //Task.Run(async () =>
        //{
        //	while (true)
        //	{
        //		var changes = await ReadWithTimeoutAsync<ChangesSignalRDTO>(signalRChannel, TimeSpan.FromMilliseconds(1000), default);
        //		var logs = changes.SelectMany(x => x.Logs).ToArray();
        //		var activityStarts = changes.SelectMany(x => x.ActivityStarts).ToArray();
        //		var activityEnds = changes.SelectMany(x => x.ActivityEnds).ToArray();
        //		var aggregatedChanges = new ChangesSignalRDTO(logs, activityStarts, activityEnds);
        //		await hubConnection.Call.ReceiveChangesFromProducer(aggregatedChanges);
        //	}

        //});
        Task.Run(async () =>
        {
            while (true)
            {
                var aggregatedChanges = await signalRChannel.Reader.ReadAsync();
                await hubConnection.Call.ReceiveChangesFromProducer(aggregatedChanges);
            }
        });
        return true;
    }

    public async Task ProduceLog(LogEntry logEntry)
    {
        var logs = new LogEntrySignalRDTO[] { LogEntrySignalRDTO.FromEntry(logEntry) };
        await signalRChannel.Writer.WriteAsync(new(logs, Array.Empty<ActivityStartSignalRDTO>(), Array.Empty<ActivityEndSignalRDTO>()));
    }

    public async Task StartActivity(ActivityStart activityStartEntry)
    {
        var starts = new ActivityStartSignalRDTO[] { ActivityStartSignalRDTO.FromEntry(activityStartEntry) };
        await signalRChannel.Writer.WriteAsync(new(Array.Empty<LogEntrySignalRDTO>(), starts, Array.Empty<ActivityEndSignalRDTO>()));

    }

    public async Task EndActivity(ActivityEnd activity)
    {
        var ends = new ActivityEndSignalRDTO[] { ActivityEndSignalRDTO.FromEntry(activity) };
        await signalRChannel.Writer.WriteAsync(new(Array.Empty<LogEntrySignalRDTO>(), Array.Empty<ActivityStartSignalRDTO>(), ends));
    }

    private static async Task<List<T>> ReadWithTimeoutAsync<T>(ChannelReader<T> reader, TimeSpan readTOut, CancellationToken cancellationToken)
    {
        var timeoutTokenSrc = new CancellationTokenSource();
        timeoutTokenSrc.CancelAfter(readTOut);

        var messages = new List<T>();

        using (CancellationTokenSource linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(timeoutTokenSrc.Token, cancellationToken))
        {
            try
            {
                await foreach (var item in reader.ReadAllAsync(linkedCts.Token))
                {
                    messages.Add(item);
                    //linkedCts.Token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                //cancellationToken.ThrowIfCancellationRequested();
            }
        }

        timeoutTokenSrc.Dispose();
        return messages;
    }
}
