using Basyc.MessageBus.Manager.Application.Initialization;
using System.Collections.Generic;

namespace Basyc.MessageBus.Manager.Application.Requesting
{
	public class RequesterSelectorOptions
	{
		Dictionary<RequestInfo, string> requesterNameToRequestInfoMap = new();
		public void AssignRequester(RequestInfo requestInfo, string requesterUniqueName)
		{
			requesterNameToRequestInfoMap.Add(requestInfo, requesterUniqueName);
		}

		public Dictionary<RequestInfo, string> ResolveRequesterMap()
		{
			return requesterNameToRequestInfoMap;
		}
	}
}
