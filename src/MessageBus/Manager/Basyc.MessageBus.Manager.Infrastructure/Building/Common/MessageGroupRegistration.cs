namespace Basyc.MessageBus.Manager.Infrastructure.Building.Common;

public record MessageGroupRegistration(string Name)
{
    public ICollection<MessageRegistration> MessageRegistrations { get; init; } = new List<MessageRegistration>();
}
