using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Application.Initialization;

#pragma warning disable CA1002 // Do not expose generic lists

public interface IMessageInfoProvider
{
    List<MessageGroup> GetMessageInfos();
}
