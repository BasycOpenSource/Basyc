namespace Basyc.MessageBus.Manager.Infrastructure.Building.Interface;

public class InterfaceDomainProviderOptions
{
    public ICollection<InterfaceRegistration> InterfaceRegistrations { get; init; } = new List<InterfaceRegistration>();
}
