namespace Basyc.MessageBus.Manager.Infrastructure.Building.Common;

public record CommonMessageInfoProviderOptions
{
    public ICollection<MessageGroupRegistration> MessageGroupRegistration { get; init; } = new List<MessageGroupRegistration>();
}
