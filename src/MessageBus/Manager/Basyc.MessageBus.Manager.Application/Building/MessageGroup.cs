namespace Basyc.MessageBus.Manager.Application.Building;

public record MessageGroup(string Name, IReadOnlyCollection<MessageInfo> Messages);
