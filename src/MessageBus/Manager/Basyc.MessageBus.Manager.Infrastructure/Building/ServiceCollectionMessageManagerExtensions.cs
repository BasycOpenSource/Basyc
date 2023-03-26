using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Infrastructure;
using Basyc.MessageBus.Manager.Infrastructure.Building.Common;
using Basyc.MessageBus.Manager.Infrastructure.Building.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager;

public static class ServiceCollectionMessageManagerExtensions
{
	public static BusManagerApplicationBuilder AddBasycBusManager(this IServiceCollection services)
	{
		services.AddSingleton<IRequestManager, RequestManager>();

		services.AddSingleton<IRequestDiagnosticsRepository, RequestDiagnosticsRepository>();

		services.AddSingleton<InMemoryRequestHandler>();
		services.AddSingleton<IRequestHandler, InMemoryRequestHandler>(x => x.GetRequiredService<InMemoryRequestHandler>());

		services.AddSingleton<InMemoryRequestDiagnosticsSource>();
		services.AddSingleton<IRequestDiagnosticsSource, InMemoryRequestDiagnosticsSource>(serviceProvider =>
			serviceProvider.GetRequiredService<InMemoryRequestDiagnosticsSource>());

		services.AddSingleton<IRequesterSelector, RequesterSelector>();
		services.AddSingleton<IRequestInfoTypeStorage, InMemoryRequestInfoTypeStorage>();

		services.AddSingleton<IMessagesInfoProvidersAggregator, MessagesInfoProvidersAggregator>();
		services.AddSingleton<IMessageInfoProvider, CommonMessageInfoProvider>();
		services.AddSingleton<IMessageInfoProvider, InterfaceMessageInfoProvider>();

		return new BusManagerApplicationBuilder(services);
	}
}
