using Basyc.MessageBus.HttpProxy.Shared.SignalR;
using Basyc.MessageBus.Shared;
using OneOf;

namespace Basyc.MessageBus.HttpProxy.Client.SignalR.Sessions;

public readonly struct SignalRSession
{
    private readonly TaskCompletionSource<OneOf<ResponseSignalRDto, ErrorMessage>> taskSource;

    public SignalRSession(string sessionId, string traceId)
    {
        SessionId = sessionId;
        TraceId = traceId;
        taskSource = new TaskCompletionSource<OneOf<ResponseSignalRDto, ErrorMessage>>();
    }

    public string SessionId { get; }

    public string TraceId { get; }

    public void Complete(OneOf<ResponseSignalRDto, ErrorMessage> result) => taskSource.SetResult(result);

    public Task<OneOf<ResponseSignalRDto, ErrorMessage>> WaitForCompletion() => taskSource.Task;
}
