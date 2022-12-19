using Basyc.Diagnostics.Producing.Shared;
using Basyc.Diagnostics.Producing.Shared.Building;
using Basyc.Diagnostics.Producing.SignalR;
using Basyc.Diagnostics.Producing.SignalR.Shared;
using Basyc.Diagnostics.Receiving.SignalR;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class SetupSignalRLogProducerStageExtensions
{
    public static SetupProducersStage AddSignalRExporter(this SetupProducersStage parent, string signalRServerRootUri = "https://localhost:44310")
    {
        parent.services.Configure<SignalRLogReceiverOptions>(x =>
        {
            x.SignalRServerUri = signalRServerRootUri + SignalRConstants.ProducersHubPattern;
        });
        return AddSignalRExporter(parent);
    }

    public static SetupProducersStage AddSignalRExporter(this SetupProducersStage parent, Action<SignalRLogReceiverOptions> optionSetup)
    {
        parent.services.Configure<SignalRLogReceiverOptions>(optionSetup);
        return AddSignalRExporter(parent);
    }

    public static SetupProducersStage AddSignalRExporter(this SetupProducersStage parent, IConfiguration configuration)
    {
        var sec = configuration.GetSection(nameof(SignalRLogReceiverOptions));
        parent.services.Configure<SignalRLogReceiverOptions>(sec, o =>
        {
            o.ErrorOnUnknownConfiguration = true;
        });
        return AddSignalRExporter(parent);
    }

    internal static SetupProducersStage AddSignalRExporter(this SetupProducersStage parent)
    {
        parent.services.AddSingleton<IDiagnosticsExporter, SignalRDiagnosticsExporter>();
        return parent;
    }
}