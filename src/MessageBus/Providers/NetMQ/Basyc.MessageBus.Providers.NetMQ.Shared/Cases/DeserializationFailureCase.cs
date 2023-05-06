namespace Basyc.MessageBus.NetMQ.Shared.Cases;

public record DeserializationFailureCase(int SessionId, string TraceId, string ParentSpanId, MessageCase MessageCase, string MessageType, Exception? Exception,
        string ErrorMessage)
    : CaseBase(SessionId, TraceId, ParentSpanId);
