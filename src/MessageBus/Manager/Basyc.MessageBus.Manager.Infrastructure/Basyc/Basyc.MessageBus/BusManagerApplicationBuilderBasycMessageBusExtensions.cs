using Basyc.MessageBus.Manager.Infrastructure.Basyc.Basyc.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

public static class BusManagerApplicationBuilderBasycMessageBusExtensions
{
    public static SetupBasycDiagnosticsReceiverMapper AddRequestHandler(this BusManagerApplicationBuilder parent) =>
        // parent.services.TryAddSingleton<IRequester, BasycTypedMessageBusRequester>();
        new(parent.Services);
}
