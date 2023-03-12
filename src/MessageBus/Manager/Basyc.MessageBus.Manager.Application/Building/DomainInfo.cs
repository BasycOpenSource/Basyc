namespace Basyc.MessageBus.Manager.Application.Building;

public record DomainInfo(string DomainName, IReadOnlyCollection<MessageInfo> Requests);
