using Microsoft.AspNetCore.SignalR.Client;

namespace Basyc.Extensions.SignalR.Client;

internal abstract class StrongTypedHubConnectionBase : IStrongTypedHubConnectionBase, IAsyncDisposable
{
    public HubConnection UnderlyingHubConnection { get; }

    public StrongTypedHubConnectionBase(HubConnection hubConnection)
    {
        UnderlyingHubConnection = hubConnection;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return UnderlyingHubConnection.StartAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return UnderlyingHubConnection.DisposeAsync();
    }
}

internal class StrongTypedHubConnectionPusher<TMethodsClientCanCall> : StrongTypedHubConnectionBase, IStrongTypedHubConnectionPusher<TMethodsClientCanCall>
{
    public TMethodsClientCanCall Call { get; }

    public StrongTypedHubConnectionPusher(TMethodsClientCanCall clientMethods, HubConnection hubConnection) : base(hubConnection)
    {
        Call = clientMethods;
    }
}

internal class StrongTypedHubConnectionReceiver<TMethodsServerCanCall> : StrongTypedHubConnectionBase, IStrongTypedHubConnectionReceiver<TMethodsServerCanCall>
{
    public StrongTypedHubConnectionReceiver(HubConnection hubConnection, TMethodsServerCanCall serverMethods) : base(hubConnection)
    {
        OnMultipleExtension.OnMultiple<TMethodsServerCanCall>(hubConnection, serverMethods);
    }
}

internal class StrongTypedHubConnectionPusherAndReceiver<TMethodsClientCanCall, TMethodsServerCanCall> : StrongTypedHubConnectionPusher<TMethodsClientCanCall>, IStrongTypedHubConnectionReceiver<TMethodsServerCanCall>, IStrongTypedHubConnectionPusherAndReceiver<TMethodsClientCanCall, TMethodsServerCanCall>
{
    public StrongTypedHubConnectionPusherAndReceiver(TMethodsClientCanCall clientMethods, HubConnection hubConnection, TMethodsServerCanCall serverMethods) : base(clientMethods, hubConnection)
    {
        OnMultipleExtension.OnMultiple<TMethodsServerCanCall>(hubConnection, serverMethods);
    }
}
