using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Application;

public interface IMessagesInfoProvidersAggregator
{
    IReadOnlyList<MessageGroup> GetMessageGroups();
}
