using Basyc.MessageBus.Manager.Application.Initialization;
using System.Collections.Generic;
using System.Linq;

namespace Basyc.MessageBus.Manager.Application
{
    public class Request
    {
        public RequestInfo RequestInfo { get; init; }
        public IReadOnlyCollection<Parameter> Parameters { get; init; }

        public Request(RequestInfo requestInfo, IEnumerable<Parameter> parameters)
        {
            RequestInfo = requestInfo;
            Parameters = parameters.ToList();
        }
    }
}