using Basyc.MessageBus.Manager.Application.Building;
using Basyc.MessageBus.Manager.Application.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Infrastructure;

public interface IRequestInfoTypeStorage
{
    void AddRequest(MessageInfo requestInfo, Type requestType);
    Type GetRequestType(MessageInfo requestInfo);

}
