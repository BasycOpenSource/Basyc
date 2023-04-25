using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.MessageBus.Broker.NetMQ.Building;

namespace Microsoft.Extensions.DependencyInjection;

public static class BasycSelectDiagnosticStageExtensions
{
    public static void UseBasycDiagnosticsProducer(this SelectDiagnosticStage selectDiagnosticStage)
    {
        if (selectDiagnosticStage.Services.Any(x => x.ServiceType == typeof(IDiagnosticsExporter)) is false)
            throw new InvalidOperationException("Need to register basyc diagnostics first");
    }
}
