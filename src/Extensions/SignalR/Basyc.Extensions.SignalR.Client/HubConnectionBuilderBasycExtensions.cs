﻿using Basyc.Extensions.SignalR.Client;
using Castle.DynamicProxy;

namespace Microsoft.AspNetCore.SignalR.Client;

public static class HubConnectionBuilderBasycExtensions
{
    private static readonly ProxyGenerator proxyGenerator = new();

    public static IStrongTypedHubConnectionBase BuildStrongTypedReceiver<TMethodsServerCanCall>(this IHubConnectionBuilder hubConnectionBuilder,
        TMethodsServerCanCall serverMethods)
    {
        var connection = hubConnectionBuilder.Build();
        return new StrongTypedHubConnectionReceiver<TMethodsServerCanCall>(connection, serverMethods);
    }

    public static IStrongTypedHubConnectionPusher<TMethodsClientCanCall> BuildStrongTyped<TMethodsClientCanCall>(
        this IHubConnectionBuilder hubConnectionBuilder)
        where TMethodsClientCanCall : class
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        CreateMethodsClientCanCallProxy<TMethodsClientCanCall>(hubConnectionBuilder, out var connection, out var hubClientProxy);
#pragma warning restore CA2000 // Dispose objects before losing scope
        return new StrongTypedHubConnectionPusher<TMethodsClientCanCall>(hubClientProxy, connection);
    }

    public static IStrongTypedHubConnectionPusherAndReceiver<TMethodsClientCanCall, TMethodsServerCanCall> BuildStrongTyped<TMethodsClientCanCall,
        TMethodsServerCanCall>(this IHubConnectionBuilder hubConnectionBuilder, TMethodsServerCanCall serverMethods)
        where TMethodsClientCanCall : class
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        CreateMethodsClientCanCallProxy<TMethodsClientCanCall>(hubConnectionBuilder, out var connection, out var hubClientProxy);
        return new StrongTypedHubConnectionPusherAndReceiver<TMethodsClientCanCall, TMethodsServerCanCall>(hubClientProxy, connection, serverMethods);
#pragma warning restore CA2000 // Dispose objects before losing scope

    }

    private static void CreateMethodsClientCanCallProxy<TMethodsClientCanCall>(
        IHubConnectionBuilder hubConnectionBuilder,
        out HubConnection connection,
        out TMethodsClientCanCall hubClientProxy)
        where TMethodsClientCanCall : class
    {
        connection = hubConnectionBuilder.Build();
        var hubClientInterceptor = new HubClientInterceptor(connection, typeof(TMethodsClientCanCall));
        hubClientProxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<TMethodsClientCanCall>(hubClientInterceptor);
    }
}
