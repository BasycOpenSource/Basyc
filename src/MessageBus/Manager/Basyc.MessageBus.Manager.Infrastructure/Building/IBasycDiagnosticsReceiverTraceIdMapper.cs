namespace Basyc.MessageBus.Manager.Infrastructure.Building;

public interface IBasycDiagnosticsReceiverTraceIdMapper
{
    string GetTraceId(string traceId);
}
