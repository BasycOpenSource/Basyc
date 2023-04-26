namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedDomainProviderOptions
{
    public ICollection<TypedDomainSettings> TypedDomainOptions { get; init; } = new List<TypedDomainSettings>();
}
