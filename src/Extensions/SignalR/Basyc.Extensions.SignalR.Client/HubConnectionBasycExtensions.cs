using Basyc.Extensions.SignalR.Client;
using Basyc.Extensions.SignalR.Client.OnMultiple;
using Castle.DynamicProxy;

namespace Microsoft.AspNetCore.SignalR.Client;

public static class HubConnectionBasycExtensions
{
    private static readonly ProxyGenerator proxyGenerator = new();

    public static IStrongTypedHubConnectionPusher<TMethodsClientCanCall> CreateStrongTyped<TMethodsClientCanCall>(this HubConnection connection)
        where TMethodsClientCanCall : class
    {
        var receiverType = typeof(TMethodsClientCanCall);
        var hubClientInterceptor = new HubClientInterceptor(connection, receiverType);
        var hubClientProxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<TMethodsClientCanCall>(hubClientInterceptor);
        return new StrongTypedHubConnectionPusher<TMethodsClientCanCall>(hubClientProxy, connection);
    }

    public static IStrongTypedHubConnectionPusher<TMethodsClientCanCall> CreateStrongTyped<TMethodsClientCanCall>(this HubConnection connection,
        TMethodsClientCanCall beforeMethods)
        where TMethodsClientCanCall : class
    {
        var receiverType = typeof(TMethodsClientCanCall);
        var hubClientInterceptor = new HubClientInterceptor(connection, receiverType, true);
        var hubClientProxy = proxyGenerator.CreateClassProxyWithTarget(beforeMethods, hubClientInterceptor);

        return new StrongTypedHubConnectionPusher<TMethodsClientCanCall>(hubClientProxy, connection);
    }

    public static IStrongTypedHubConnectionReceiver<TMethodsServerCanCall> CreateStrongTypedReceiver<TMethodsServerCanCall>(this HubConnection connection,
        TMethodsServerCanCall methodsServerCanCall) => new StrongTypedHubConnectionReceiver<TMethodsServerCanCall>(connection, methodsServerCanCall);

    public static IStrongTypedHubConnectionPusher<TMethodsClientCanCall> CreateStrongTyped<TMethodsClientCanCall, TMethodsServerCanCall>(
        this HubConnection connection, TMethodsServerCanCall methodsServerCanCall)
        where TMethodsClientCanCall : class
    {
        var hubClientInterceptor = new HubClientInterceptor(connection, typeof(TMethodsClientCanCall));
        var hubClientProxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<TMethodsClientCanCall>(hubClientInterceptor);
        return new StrongTypedHubConnectionPusherAndReceiver<TMethodsClientCanCall, TMethodsServerCanCall>(hubClientProxy, connection, methodsServerCanCall);
    }

    public static OnMultipleSubscription OnMultiple<TMethodsServerCanCall>(this HubConnection hubConnection, TMethodsServerCanCall methodsServerCanCall) =>
        OnMultipleExtension.OnMultiple(hubConnection, methodsServerCanCall);
}
