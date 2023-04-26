using Microsoft.AspNetCore.SignalR.Client;

namespace Basyc.Extensions.SignalR.Client;
#pragma warning disable SA1402 // File may only contain a single type

internal abstract class StrongTypedHubConnectionBase : IStrongTypedHubConnectionBase, IAsyncDisposable
{
    public StrongTypedHubConnectionBase(HubConnection hubConnection)
    {
        UnderlyingHubConnection = hubConnection;
    }

    public HubConnection UnderlyingHubConnection { get; }

    public Task StartAsync(CancellationToken cancellationToken = default) => UnderlyingHubConnection.StartAsync(cancellationToken);

    public ValueTask DisposeAsync() => UnderlyingHubConnection.DisposeAsync();
}

internal class StrongTypedHubConnectionPusher<TMethodsClientCanCall> : StrongTypedHubConnectionBase, IStrongTypedHubConnectionPusher<TMethodsClientCanCall>
{
    public StrongTypedHubConnectionPusher(TMethodsClientCanCall clientMethods, HubConnection hubConnection) : base(hubConnection)
    {
        Call = clientMethods;
    }

    public TMethodsClientCanCall Call { get; }
}

internal class StrongTypedHubConnectionReceiver<TMethodsServerCanCall> : StrongTypedHubConnectionBase, IStrongTypedHubConnectionReceiver<TMethodsServerCanCall>
{
    public StrongTypedHubConnectionReceiver(HubConnection hubConnection, TMethodsServerCanCall serverMethods) : base(hubConnection)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        OnMultipleExtension.OnMultiple(hubConnection, serverMethods);
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}

internal class StrongTypedHubConnectionPusherAndReceiver<TMethodsClientCanCall, TMethodsServerCanCall> : StrongTypedHubConnectionPusher<TMethodsClientCanCall>, IStrongTypedHubConnectionReceiver<TMethodsServerCanCall>, IStrongTypedHubConnectionPusherAndReceiver<TMethodsClientCanCall, TMethodsServerCanCall>
{
    public StrongTypedHubConnectionPusherAndReceiver(TMethodsClientCanCall clientMethods, HubConnection hubConnection, TMethodsServerCanCall serverMethods) : base(clientMethods, hubConnection)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        OnMultipleExtension.OnMultiple(hubConnection, serverMethods);
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}
