using Basyc.MessageBus.Manager.Application.Initialization;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Application.Requesting
{
    public interface IRequestManager
    {
        RequestContext StartRequest(Request request);
        Dictionary<RequestInfo, List<RequestContext>> Results { get; }
    }
}