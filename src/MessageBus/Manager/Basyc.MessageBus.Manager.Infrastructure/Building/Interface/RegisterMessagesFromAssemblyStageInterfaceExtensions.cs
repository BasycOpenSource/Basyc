using Basyc.MessageBus.Manager.Infrastructure.Building;
using Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

namespace Microsoft.Extensions.DependencyInjection;

public static class RegisterMessagesFromAssemblyStageInterfaceExtensions
{
    public static SetupDisplayNameStage FromInterface<TInterfaceMessage>(this RegisterMessagesFromAssemblyStage fromAssemblyStag) => FromInterface(fromAssemblyStag, typeof(TInterfaceMessage));

    public static SetupDisplayNameStage FromInterface(this RegisterMessagesFromAssemblyStage fromAssemblyStage, Type interfaceType)
    {
        var interfaceRegistration = new InterfaceRegistration();
        interfaceRegistration.AssembliesToScan.AddRange(fromAssemblyStage.assembliesToScan);
        interfaceRegistration.MessageInterfaceType = interfaceType;
        interfaceRegistration.GroupName = fromAssemblyStage.groupName;
        interfaceRegistration.DisplayNameFormatter = x => x.Name;
        fromAssemblyStage.services.Configure<InterfaceDomainProviderOptions>(options =>
        {
            options.InterfaceRegistrations.Add(interfaceRegistration);
        });
        return new SetupDisplayNameStage(fromAssemblyStage.services, interfaceRegistration);
    }
}
