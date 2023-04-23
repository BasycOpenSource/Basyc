using Basyc.MessageBus.Manager.Infrastructure.Building;

namespace Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;

public class BusManagerBasycDiagnosticsReceiverTraceIdMapper : IBasycDiagnosticsReceiverTraceIdMapper
{
    private readonly Dictionary<string, string> foreinfIdToSessionIdMap = new();

    public string GetTraceId(string traceId) => foreinfIdToSessionIdMap[traceId];

    public void AddMapping(string traceId, string foreingId) => foreinfIdToSessionIdMap.Add(foreingId, traceId);
}
