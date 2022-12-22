namespace Basyc.MessageBus.Client.NetMQ.Sessions;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TSessionResult"></typeparam>
/// <param name="SessionId">Id used to internal handeleing. This id should not be propageded outside the internal infrastrucuture</param>
/// <param name="TraceId"></param>
/// <param name="ParentSpanId"></param>
/// <param name="MessageType"></param>
/// <param name="ResponseSource"></param>
public record NetMqSession<TSessionResult>(int SessionId, string? TraceId, string MessageType, TaskCompletionSource<TSessionResult> ResponseSource)
{
	public const int UnknownSessionId = -1;
}