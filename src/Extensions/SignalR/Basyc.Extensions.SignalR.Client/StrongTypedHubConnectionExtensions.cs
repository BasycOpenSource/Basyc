using System.Linq.Expressions;

namespace Basyc.Extensions.SignalR.Client;

public static class StrongTypedHubConnectionExtensions
{

    public static void OnMultiple<TMethodsServerCanCall>(this IStrongTypedHubConnectionReceiver<TMethodsServerCanCall> hubConnection, TMethodsServerCanCall methodsServerCanCall)
    {
        OnMultipleExtension.OnMultiple(hubConnection.UnderlyingHubConnection, methodsServerCanCall);
    }

    public static Task Receive<TServerCanCall, TMethodToWait>(this IStrongTypedHubConnectionReceiver<TServerCanCall> strongTypedHubConnection, Expression<Func<TServerCanCall, TMethodToWait>> methodSelector, TMethodToWait handler)
        where TServerCanCall : notnull
        where TMethodToWait : Delegate
    {
        return ReceiveExtension.Receive(strongTypedHubConnection, methodSelector, handler);
    }
}
