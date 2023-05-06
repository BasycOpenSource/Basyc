using Basyc.MessageBus.Manager.Infrastructure.Building;
using Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

namespace Microsoft.Extensions.DependencyInjection;

public static class RegisterMessagesFromAssemblyStageInterfaceExtensions
{
    public static SetupDisplayNameStage FromInterface<TInterfaceMessage>(this RegisterMessagesFromAssemblyStage fromAssemblyStag) => FromInterface(fromAssemblyStag, typeof(TInterfaceMessage));

    public static SetupDisplayNameStage FromInterface(this RegisterMessagesFromAssemblyStage fromAssemblyStage, Type interfaceType)
    {
        var interfaceRegistration = new InterfaceRegistration();
        foreach (var assembly in fromAssemblyStage.AssembliesToScan)
        {
            interfaceRegistration.AssembliesToScan.Add(assembly);
        }

        interfaceRegistration.MessageInterfaceType = interfaceType;
        interfaceRegistration.GroupName = fromAssemblyStage.GroupName;
        interfaceRegistration.DisplayNameFormatter = x => x.Name;
        fromAssemblyStage.Services.Configure<InterfaceDomainProviderOptions>(options =>
        {
            options.InterfaceRegistrations.Add(interfaceRegistration);
        });
        return new SetupDisplayNameStage(fromAssemblyStage.Services, interfaceRegistration);
    }
}
