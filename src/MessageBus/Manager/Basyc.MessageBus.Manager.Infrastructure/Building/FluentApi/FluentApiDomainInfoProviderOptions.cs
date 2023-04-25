using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

namespace Basyc.MessageBus.Manager.Application;

public class FluentApiDomainInfoProviderOptions
{
    public List<FluentApiGroupRegistration> GroupRegistrations { get; } = new();
}
