using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

namespace Microsoft.Extensions.DependencyInjection;

public static class BusManagerApplicationBuilderFluentExtensions
{
	public static RegisterMessagesFromFluentApiStage RegisterMessagesFromFluentApi(this BusManagerApplicationBuilder parent)
	{
		//parent.services.AddSingleton<IRequesterSelector, RequesterSelector>();
		parent.services.AddSingleton<IRequester, InMemoryDelegateRequester>();
		parent.services.AddSingleton<InMemoryDelegateRequester>();
		//parent.services.AddSingleton<InMemoryRequestDiagnosticsSource>();
		//parent.services.AddSingleton<IRequestDiagnosticsSource>(serviceProvider => serviceProvider.GetRequiredService<InMemoryRequestDiagnosticsSource>());
		parent.services.AddSingleton<IDomainInfoProvider, FluentApiDomainInfoProvider>();

		return new RegisterMessagesFromFluentApiStage(parent.services);
	}
}
