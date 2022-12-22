using Basyc.Diagnostics.Receiving.Abstractions.Building;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionDiagnosticsExtensions
{
	public static SelectReceiverProviderStage AddBasycDiagnosticReceiving(this IServiceCollection services)
	{
		return new SelectReceiverProviderStage(services);
	}
}