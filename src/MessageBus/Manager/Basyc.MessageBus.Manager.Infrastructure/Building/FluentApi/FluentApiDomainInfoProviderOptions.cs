using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

namespace Basyc.MessageBus.Manager.Application;

public class FluentApiDomainInfoProviderOptions
{
    public ICollection<FluentApiGroupRegistration> GroupRegistrations { get; init; } = new List<FluentApiGroupRegistration>();
}
