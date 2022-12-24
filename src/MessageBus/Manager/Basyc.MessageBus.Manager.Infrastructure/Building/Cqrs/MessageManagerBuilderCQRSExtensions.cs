using Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Infrastructure;
using Basyc.MessageBus.Manager.Infrastructure.Building;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class MessageManagerBuilderCQRSExtensions
{
	public static SetupDiagnosticsStage RegisterMessagesAsCQRS(this RegisterMessagesFromAssemblyStage parentStage, Type iQueryType, Type iCommandType, Type iCommandWithResponseType)
	{
		parentStage.services.Configure<CqrsInterfacedDomainProviderOptions>(options =>
		{
			options.AddCQRSRegistration(new CqrsInterfacedDomainProviderOptions.CQRSRegistration()
			{
				IQueryType = iQueryType,
				ICommandType = iCommandType,
				ICommandWithResponseType = iCommandWithResponseType,
				AssembliesToScan = parentStage.assembliesToScan,
				DomainName = parentStage.domainName,
			});
		});
		parentStage.services.AddSingleton<IDomainInfoProvider, CqrsInterfacedDomainProvider>();
		return new SetupDiagnosticsStage(parentStage.services);

	}

	public static SetupTypeFormattingStage RegisterMessages(this RegisterMessagesFromAssemblyStage fromAssemblyStage, Type iMessageType, Type iMessageWithResponseType)
	{
		fromAssemblyStage.services.Configure<CqrsInterfacedDomainProviderOptions>(options =>
		{
			options.AddCQRSRegistration(new CqrsInterfacedDomainProviderOptions.CQRSRegistration()
			{
				IMessageType = iMessageType,
				IMessageWithResponseType = iMessageWithResponseType,
				AssembliesToScan = fromAssemblyStage.assembliesToScan,
			});
		});
		fromAssemblyStage.services.AddSingleton<IDomainInfoProvider, CqrsInterfacedDomainProvider>();
		return new SetupTypeFormattingStage(fromAssemblyStage.services);
	}
}
