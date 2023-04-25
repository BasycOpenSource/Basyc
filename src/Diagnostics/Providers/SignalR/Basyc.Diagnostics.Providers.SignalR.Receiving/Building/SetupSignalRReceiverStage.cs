using Basyc.DependencyInjection;
using Basyc.Diagnostics.Producing.SignalR.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Diagnostics.Receiving.SignalR.Building;

public class SetupSignalRReceiverStage : BuilderStageBase
{
    public SetupSignalRReceiverStage(IServiceCollection services) : base(services)
    {
    }

    public SetupSignalRReceiverStage UseConfiguration(IConfiguration configuration)
    {
        var sec = configuration.GetSection(nameof(SignalRLogReceiverOptions));
        Services.Configure<SignalRLogReceiverOptions>(sec, o =>
        {
            o.ErrorOnUnknownConfiguration = true;
        });
        return this;
    }

    public void SetServerUri(string serverUri, string receiverHubPattern = SignalRConstants.ReceiversHubPattern) => Services.Configure<SignalRLogReceiverOptions>(options =>
                                                                                                                         {
                                                                                                                             options.SignalRServerReceiverHubUri = serverUri + receiverHubPattern;
                                                                                                                         });
}
