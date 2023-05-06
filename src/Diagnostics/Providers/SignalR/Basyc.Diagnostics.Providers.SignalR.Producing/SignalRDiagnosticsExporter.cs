using System.Threading.Channels;
using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Providers.SignalR.Shared.DTOs;
using Basyc.Diagnostics.Receiving.SignalR;
using Basyc.Diagnostics.Shared.Logging;
using Basyc.Diagnostics.SignalR.Shared;
using Basyc.Diagnostics.SignalR.Shared.DTOs;
using Basyc.Extensions.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace Basyc.Diagnostics.Producing.SignalR;

public class SignalRDiagnosticsExporter : IDiagnosticsExporter
{
    private readonly IStrongTypedHubConnectionPusher<IServerMethodsProducersCanCall> hubConnection;

    private readonly Channel<ChangesSignalRDto>
        signalRChannel = Channel.CreateUnbounded<ChangesSignalRDto>(new()
        {
            SingleReader = true
        });

    public SignalRDiagnosticsExporter(IOptions<SignalRLogReceiverOptions> options)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(options.Value.SignalRServerUri!)
            .WithAutomaticReconnect()
            .BuildStrongTyped<IServerMethodsProducersCanCall>();
    }

    /// <summary>
    ///     Returns false when failed to connect.
    /// </summary>
    public async Task<bool> StartAsync()
    {
        await hubConnection.StartAsync();

        // Task.Run(async () =>
        // {
        //      while (true)
        //  {
        //  var changes = await ReadWithTimeoutAsync<ChangesSignalRDTO>(signalRChannel, TimeSpan.FromMilliseconds(1000), default);
        //  var logs = changes.SelectMany(x => x.Logs).ToArray();
        //  var activityStarts = changes.SelectMany(x => x.ActivityStarts).ToArray();
        //  var activityEnds = changes.SelectMany(x => x.ActivityEnds).ToArray();
        //  var aggregatedChanges = new ChangesSignalRDTO(logs, activityStarts, activityEnds);
        //  await hubConnection.Call.ReceiveChangesFromProducer(aggregatedChanges);
        //  }
        // });
#pragma warning disable CS4014
        Task.Run(async () =>
#pragma warning restore CS4014
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
        var logs = new[] { LogEntrySignalRDto.FromEntry(logEntry) };
        await signalRChannel.Writer.WriteAsync(new(logs, Array.Empty<ActivityStartSignalRDto>(), Array.Empty<ActivityEndSignalRDto>()));
    }

    public async Task StartActivity(ActivityStart activityStart)
    {
        var starts = new[] { ActivityStartSignalRDto.FromEntry(activityStart) };
        await signalRChannel.Writer.WriteAsync(new(Array.Empty<LogEntrySignalRDto>(), starts, Array.Empty<ActivityEndSignalRDto>()));
    }

    public async Task EndActivity(ActivityEnd activityEnd)
    {
        var ends = new[] { ActivityEndSignalRDto.FromEntry(activityEnd) };
        await signalRChannel.Writer.WriteAsync(new(Array.Empty<LogEntrySignalRDto>(), Array.Empty<ActivityStartSignalRDto>(), ends));
    }

#pragma warning disable IDE0051 // Remove unused private members
    private static async Task<List<T>> ReadWithTimeoutAsync<T>(ChannelReader<T> reader, TimeSpan readTOut, CancellationToken cancellationToken)
#pragma warning restore IDE0051 // Remove unused private members
    {
        var timeoutTokenSrc = new CancellationTokenSource();
        timeoutTokenSrc.CancelAfter(readTOut);

        var messages = new List<T>();

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutTokenSrc.Token, cancellationToken);
        try
        {
            await foreach (var item in reader.ReadAllAsync(linkedCts.Token))
                messages.Add(item);
            // linkedCts.Token.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException)
        {
            // cancellationToken.ThrowIfCancellationRequested();
        }

        timeoutTokenSrc.Dispose();
        return messages;
    }
}
