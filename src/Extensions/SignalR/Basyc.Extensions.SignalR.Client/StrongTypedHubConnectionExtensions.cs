using System.Linq.Expressions;

namespace Basyc.Extensions.SignalR.Client;

public static class StrongTypedHubConnectionExtensions
{
#pragma warning disable CA2000 // Dispose objects before losing scope
    public static void OnMultiple<TMethodsServerCanCall>(this IStrongTypedHubConnectionReceiver<TMethodsServerCanCall> hubConnection, TMethodsServerCanCall methodsServerCanCall) => OnMultipleExtension.OnMultiple(hubConnection.UnderlyingHubConnection, methodsServerCanCall);
#pragma warning restore CA2000 // Dispose objects before losing scope

    public static Task Receive<TServerCanCall, TMethodToWait>(this IStrongTypedHubConnectionReceiver<TServerCanCall> strongTypedHubConnection, Expression<Func<TServerCanCall, TMethodToWait>> methodSelector, TMethodToWait handler)
        where TServerCanCall : notnull
        where TMethodToWait : Delegate => ReceiveExtension.Receive(strongTypedHubConnection, methodSelector, handler);
}
