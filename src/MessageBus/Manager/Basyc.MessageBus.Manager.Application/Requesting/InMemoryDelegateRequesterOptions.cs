using Basyc.MessageBus.Manager.Application.Initialization;
using System;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Application.Requesting
{
	public class InMemoryDelegateRequesterOptions
	{
		private readonly Dictionary<RequestInfo, Action<RequestContext>> handlerMap = new();

		public void AddDelegateHandler(RequestInfo requestInfo, Action<RequestContext> handler)
		{
			handlerMap.Add(requestInfo, handler);
		}

		public Dictionary<RequestInfo, Action<RequestContext>> ResolveHandlers()
		{
			return handlerMap;
		}

	}
}