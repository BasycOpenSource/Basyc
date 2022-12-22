namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public class NullBasycDiagnosticsReceiverTraceIdMapper : IBasycDiagnosticsReceiverTraceIdMapper
{
    public string GetTraceId(string traceId)
    {
        return traceId;
    }
}
