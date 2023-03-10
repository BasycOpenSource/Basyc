using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure;
using Basyc.MessageBus.Manager.Infrastructure.Building;
using Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

namespace Microsoft.Extensions.DependencyInjection;

public class BusManagerApplicationBuilder : BuilderStageBase
{
	public BusManagerApplicationBuilder(IServiceCollection services) : base(services)
	{
		services.AddSingleton<IRequesterSelector, RequesterSelector>();
		services.AddSingleton<IDomainInfoProviderManager, DomainInfoProviderManager>();
		services.AddSingleton<IRequestInfoTypeStorage, InMemoryRequestInfoTypeStorage>();
		services.AddSingleton<IDomainInfoProvider, InterfaceDomainProvider>();
	}

	public SetupMessagesStage RegisterMessages()
	{
		return new SetupMessagesStage(services);
	}
}
