namespace Basyc.MessageBus.NetMQ.Shared.Cases;

public record RequestCase(int SessionId, string TraceId, string ParentSpanId, string RequestType, byte[] RequestBytes, bool ExpectsResponse)
	 : CaseBase(SessionId, TraceId, ParentSpanId);