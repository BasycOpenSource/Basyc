using Basyc.MessageBus.Manager.Application.Building;

namespace Basyc.MessageBus.Manager.Infrastructure;

public interface IRequestInfoTypeStorage
{
    void AddRequest(MessageInfo requestInfo, Type requestType);

    Type GetRequestType(MessageInfo requestInfo);
}
