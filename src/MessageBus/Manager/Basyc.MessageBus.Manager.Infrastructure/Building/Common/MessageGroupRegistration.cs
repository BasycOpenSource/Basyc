using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.Common;

public record MessageGroupRegistration(string Name)
{
	public List<MessageRegistration> MessageRegistrations { get; init; } = new();
}
