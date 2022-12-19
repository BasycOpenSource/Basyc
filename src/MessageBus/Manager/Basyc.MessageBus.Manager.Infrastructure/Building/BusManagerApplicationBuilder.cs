using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure;
using Basyc.MessageBus.Manager.Infrastructure.Building.Interface;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration
{
	public class BusManagerApplicationBuilder : BuilderStageBase
	{
		public BusManagerApplicationBuilder(IServiceCollection services) : base(services)
		{
			services.AddSingleton<IRequesterSelector, RequesterSelector>();
			services.AddSingleton<IDomainInfoProviderManager, DomainInfoProviderManager>();
			services.AddSingleton<IRequestInfoTypeStorage, InMemoryRequestInfoTypeStorage>();
			services.AddSingleton<IDomainInfoProvider, InterfaceDomainProvider>();


		}

		public SetupDomainStage RegisterMessagesFromAssembly(params Assembly[] assembliesToScan)
		{
			return new SetupDomainStage(services, assembliesToScan);
		}

		public void RegisterRequester<TRequester>() where TRequester : class, IRequester
		{
			services.AddSingleton<IRequester, TRequester>();
		}

	}
}