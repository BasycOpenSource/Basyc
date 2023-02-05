using Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Infrastructure;
using Basyc.MessageBus.Manager.Infrastructure.Building;

namespace Microsoft.Extensions.DependencyInjection;

public static class MessageManagerBuilderCqrsExtensions
{
	public static SetupDiagnosticsStage RegisterMessagesAsCqrs(this RegisterMessagesFromAssemblyStage parentStage, Type iQueryType, Type iCommandType,
		Type iCommandWithResponseType)
	{
		parentStage.services.Configure<CqrsInterfacedDomainProviderOptions>(options =>
		{
			options.AddcqrsRegistration(new CqrsInterfacedDomainProviderOptions.CqrsRegistration
			{
				QueryInterfaceType = iQueryType,
				CommandInterfaceType = iCommandType,
				CommandWithResponseInterfaceType = iCommandWithResponseType,
				AssembliesToScan = parentStage.assembliesToScan,
				DomainName = parentStage.domainName
			});
		});
		parentStage.services.AddSingleton<IDomainInfoProvider, CqrsInterfacedDomainProvider>();
		return new SetupDiagnosticsStage(parentStage.services);
	}

	public static SetupTypeFormattingStage RegisterMessages(this RegisterMessagesFromAssemblyStage fromAssemblyStage, Type iMessageType,
		Type iMessageWithResponseType)
	{
		fromAssemblyStage.services.Configure<CqrsInterfacedDomainProviderOptions>(options =>
		{
			options.AddcqrsRegistration(new CqrsInterfacedDomainProviderOptions.CqrsRegistration
			{
				MessageInterfaceType = iMessageType,
				MessageWithResponseInterfaceType = iMessageWithResponseType,
				AssembliesToScan = fromAssemblyStage.assembliesToScan
			});
		});
		fromAssemblyStage.services.AddSingleton<IDomainInfoProvider, CqrsInterfacedDomainProvider>();
		return new SetupTypeFormattingStage(fromAssemblyStage.services);
	}
}
