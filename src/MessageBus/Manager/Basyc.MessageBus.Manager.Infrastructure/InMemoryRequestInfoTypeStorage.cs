using Basyc.MessageBus.Manager.Application.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Infrastructure
{
    public class InMemoryRequestInfoTypeStorage : IRequestInfoTypeStorage
    {
        private readonly Dictionary<RequestInfo, Type> storage = new Dictionary<RequestInfo, Type>();

        public void AddRequest(RequestInfo requestInfo, Type requestType)
        {
            storage.Add(requestInfo, requestType);
        }

        public Type GetRequestType(RequestInfo requestInfo)
        {
            return storage[requestInfo];
        }
    }
}