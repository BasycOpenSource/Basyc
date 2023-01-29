using Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration;
using Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

namespace Microsoft.Extensions.DependencyInjection;

public static class MessageManagerBuilderInterfacedExtensionsInterfaceExntesions
{
	public static SetupDisplayNameStage FromInterface<TInterfaceMessage>(this RegisterMessagesFromAssemblyStage fromAssemblyStag)
	{
		return FromInterface(fromAssemblyStag, typeof(TInterfaceMessage));
	}

	public static SetupDisplayNameStage FromInterface(this RegisterMessagesFromAssemblyStage fromAssemblyStage, Type interfaceType)
	{
		var registration = new InterfaceRegistration();
		registration.AssembliesToScan.AddRange(fromAssemblyStage.assembliesToScan);
		registration.MessageInterfaceType = interfaceType;
		registration.DomainName = fromAssemblyStage.domainName;
		fromAssemblyStage.services.Configure<InterfaceDomainProviderOptions>(options =>
		{
			options.InterfaceRegistrations.Add(registration);
		});
		return new SetupDisplayNameStage(fromAssemblyStage.services, registration);
	}
}
