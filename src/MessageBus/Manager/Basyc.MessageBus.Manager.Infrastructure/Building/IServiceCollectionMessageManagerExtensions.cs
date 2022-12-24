using Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager;

public static class IServiceCollectionMessageManagerExtensions
{
	public static BusManagerApplicationBuilder AddBasycBusManager(this IServiceCollection services)
	{
		services.AddSingleton<IRequestManager, RequestManager>();
		services.AddSingleton<IRequestDiagnosticsManager, RequestDiagnosticsManager>();

		services.AddSingleton<InMemoryRequestDiagnosticsSource>();
		services.AddSingleton<IRequestDiagnosticsSource>(serviceProvider => serviceProvider.GetRequiredService<InMemoryRequestDiagnosticsSource>());
		var builder = new BusManagerApplicationBuilder(services);
		return builder;
	}
}
