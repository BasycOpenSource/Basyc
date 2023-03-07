using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager;

public static class ServiceCollectionMessageManagerExtensions
{
	public static BusManagerApplicationBuilder AddBasycBusManager(this IServiceCollection services)
	{
		services.AddSingleton<IRequestManager, RequestManager>();

		services.AddSingleton<IRequestDiagnosticsManager, RequestDiagnosticsManager>();

		services.AddSingleton<InMemoryRequestHandler>();
		services.AddSingleton<IRequestHandler, InMemoryRequestHandler>(x => x.GetRequiredService<InMemoryRequestHandler>());

		services.AddSingleton<InMemoryRequestDiagnosticsSource>();
		services.AddSingleton<IRequestDiagnosticsSource, InMemoryRequestDiagnosticsSource>(serviceProvider =>
			serviceProvider.GetRequiredService<InMemoryRequestDiagnosticsSource>());

		return new BusManagerApplicationBuilder(services);
	}
}
