namespace Basyc.MessageBus.Client.NetMQ.Sessions;
/// <summary>
/// NetMqSession class.
/// </summary>
/// <typeparam name="TSessionResult"></typeparam>
/// <param name="SessionId">Id used to internal handling. This id should not be propagated outside the internal infrastructure.</param>
public record NetMqSession<TSessionResult>(int SessionId, string? TraceId, string MessageType, TaskCompletionSource<TSessionResult> ResponseSource)
{
    public const int UnknownSessionId = -1;
}
