using Basyc.Diagnostics.Receiving.Abstractions.Building;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionDiagnosticsExtensions
{
    public static SelectReceiverProviderStage AddBasycDiagnosticsReceiving(this IServiceCollection services) => new(services);
}
