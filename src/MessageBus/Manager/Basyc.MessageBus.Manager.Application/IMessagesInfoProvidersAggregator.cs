using Basyc.MessageBus.Manager.Application.Building;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Application;

public interface IMessagesInfoProvidersAggregator
{
    IReadOnlyList<MessageGroup> GetMessageGroups();
}
