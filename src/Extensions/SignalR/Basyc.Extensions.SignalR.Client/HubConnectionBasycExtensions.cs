using Basyc.Extensions.SignalR.Client;
using Basyc.Extensions.SignalR.Client.OnMultiple;
using Castle.DynamicProxy;

namespace Microsoft.AspNetCore.SignalR.Client
{
    public static class HubConnectionBasycExtensions
    {
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static IStrongTypedHubConnectionPusher<TMethodsClientCanCall> CreateStrongTyped<TMethodsClientCanCall>(this HubConnection connection)
            where TMethodsClientCanCall : class
        {
            var receiverType = typeof(TMethodsClientCanCall);
            HubClientInteceptor hubClientInteceptor = new HubClientInteceptor(connection, receiverType);
            TMethodsClientCanCall hubClientProxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<TMethodsClientCanCall>(hubClientInteceptor);
            return new StrongTypedHubConnectionPusher<TMethodsClientCanCall>(hubClientProxy, connection);
        }


        public static IStrongTypedHubConnectionPusher<TMethodsClientCanCall> CreateStrongTyped<TMethodsClientCanCall>(this HubConnection connection, TMethodsClientCanCall beforeMethods)
            where TMethodsClientCanCall : class
        {
            var receiverType = typeof(TMethodsClientCanCall);
            HubClientInteceptor hubClientInteceptor = new HubClientInteceptor(connection, receiverType, true);
            TMethodsClientCanCall hubClientProxy;

            hubClientProxy = proxyGenerator.CreateClassProxyWithTarget<TMethodsClientCanCall>(beforeMethods, hubClientInteceptor);

            return new StrongTypedHubConnectionPusher<TMethodsClientCanCall>(hubClientProxy, connection);
        }

        public static IStrongTypedHubConnectionReceiver<TMethodsServerCanCall> CreateStrongTypedReceiver<TMethodsServerCanCall>(this HubConnection connection, TMethodsServerCanCall methodsServerCanCall)
        {
            return new StrongTypedHubConnectionReceiver<TMethodsServerCanCall>(connection, methodsServerCanCall);
        }

        public static IStrongTypedHubConnectionPusher<TMethodsClientCanCall> CreateStrongTyped<TMethodsClientCanCall, TMethodsServerCanCall>(this HubConnection connection, TMethodsServerCanCall methodsServerCanCall)
            where TMethodsClientCanCall : class
        {
            HubClientInteceptor hubClientInteceptor = new HubClientInteceptor(connection, typeof(TMethodsClientCanCall));
            var hubClientProxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<TMethodsClientCanCall>(hubClientInteceptor);
            return new StrongTypedHubConnectionPusherAndReceiver<TMethodsClientCanCall, TMethodsServerCanCall>(hubClientProxy, connection, methodsServerCanCall);
        }

        public static OnMultipleSubscription OnMultiple<TMethodsServerCanCall>(this HubConnection hubConnection, TMethodsServerCanCall methodsServerCanCall)
        {
            return OnMultipleExtension.OnMultiple(hubConnection, methodsServerCanCall);
        }
    }
}