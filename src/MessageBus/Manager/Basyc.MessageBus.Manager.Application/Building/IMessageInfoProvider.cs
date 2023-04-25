using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Application.Initialization;

public interface IMessageInfoProvider
{
    List<MessageGroup> GetMessageInfos();
}
