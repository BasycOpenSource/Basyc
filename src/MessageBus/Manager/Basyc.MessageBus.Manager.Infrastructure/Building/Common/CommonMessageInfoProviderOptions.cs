namespace Basyc.MessageBus.Manager.Infrastructure.Building.Common;

public record  CommonMessageInfoProviderOptions
{
	public List<MessageGroupRegistration> MessageGroupRegistration { get; } = new List<MessageGroupRegistration>();
}
