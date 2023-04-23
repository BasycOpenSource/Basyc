using Microsoft.AspNetCore.SignalR.Client;

namespace Basyc.Extensions.SignalR.Client;

public interface IStrongTypedHubConnectionBase : IAsyncDisposable
{
    /// <summary>
    ///     Gets all strongly typed messages available to call.
    /// </summary>
    HubConnection UnderlyingHubConnection { get; }

    Task StartAsync(CancellationToken cancellationToken = default);
}

public interface IStrongTypedHubConnectionPusher<TMethodsClientCanCall> : IStrongTypedHubConnectionBase
{
    /// <summary>
    ///     Gets all strongly typed messages available to call from to server.
    /// </summary>
    TMethodsClientCanCall Call { get; }
}

public interface IStrongTypedHubConnectionReceiver<TMethodsServerCanCall> : IStrongTypedHubConnectionBase
{
}

public interface IStrongTypedHubConnectionPusherAndReceiver<TMethodsClientCanCall, TMethodsServerCanCall> : IStrongTypedHubConnectionBase,
    IStrongTypedHubConnectionReceiver<TMethodsServerCanCall>
{
    /// <summary>
    ///     Gets all strongly typed messages available to call from to server.
    /// </summary>
    TMethodsClientCanCall Call { get; }
}
