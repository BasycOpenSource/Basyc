using Basyc.MessageBus.Manager.Infrastructure.Building;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.MessageBus;

public class BusManagerBasycDiagnosticsReceiverTraceIDMapper : IBasycDiagnosticsReceiverTraceIdMapper
{
    private readonly Dictionary<string, string> foreinfIdToSessionIdMap = new();
    public string GetTraceId(string traceId)
    {
        return foreinfIdToSessionIdMap[traceId];
    }

    public void AddMapping(string traceId, string foreingId)
    {
        foreinfIdToSessionIdMap.Add(foreingId, traceId);
    }
}
