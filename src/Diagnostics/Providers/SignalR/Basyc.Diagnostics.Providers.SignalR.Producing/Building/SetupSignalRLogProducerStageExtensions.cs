using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Producing.Shared.Building;
using Basyc.Diagnostics.Producing.SignalR;
using Basyc.Diagnostics.Producing.SignalR.Shared;
using Basyc.Diagnostics.Receiving.SignalR;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable CA1054 // URI-like parameters should not be strings

public static class SetupSignalRLogProducerStageExtensions
{
    public static SetupProducersStage AddSignalRExporter(this SetupProducersStage parent, string signalRServerRootUri = "https://localhost:44310")
    {
        parent.Services.Configure<SignalRLogReceiverOptions>(x =>
        {
            x.SignalRServerUri = signalRServerRootUri + SignalRConstants.ProducersHubPattern;
        });
        return AddSignalRExporter(parent);
    }

    public static SetupProducersStage AddSignalRExporter(this SetupProducersStage parent, Action<SignalRLogReceiverOptions> optionSetup)
    {
        parent.Services.Configure(optionSetup);
        return AddSignalRExporter(parent);
    }

    public static SetupProducersStage AddSignalRExporter(this SetupProducersStage parent, IConfiguration configuration)
    {
        var sec = configuration.GetSection(nameof(SignalRLogReceiverOptions));
        parent.Services.Configure<SignalRLogReceiverOptions>(sec, o =>
        {
            o.ErrorOnUnknownConfiguration = true;
        });
        return AddSignalRExporter(parent);
    }

    internal static SetupProducersStage AddSignalRExporter(this SetupProducersStage parent)
    {
        parent.Services.AddSingleton<IDiagnosticsExporter, SignalRDiagnosticsExporter>();
        return parent;
    }
}
