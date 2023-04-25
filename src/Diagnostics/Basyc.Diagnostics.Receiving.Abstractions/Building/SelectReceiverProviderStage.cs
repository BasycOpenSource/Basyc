using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Basyc.Diagnostics.Receiving.Abstractions.Building;

public class SelectReceiverProviderStage : BuilderStageBase
{
    public SelectReceiverProviderStage(IServiceCollection services) : base(services)
    {
    }

    public SetupInMemoryReceiverStage AddInMemoryReceiver()
    {
        Services.TryAddSingleton<InMemoryDiagnosticReceiver>();
        Services.AddSingleton<IDiagnosticReceiver, InMemoryDiagnosticReceiver>(x => x.GetRequiredService<InMemoryDiagnosticReceiver>());
        return new SetupInMemoryReceiverStage(Services);
    }
}
