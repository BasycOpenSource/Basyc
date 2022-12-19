using System;

namespace Basyc.MessageBus.Client
{
	[Obsolete("Not used")]
	public interface ISessionIdMapper
	{
		void MapSessionId(int requestId, int sessionId);

	}
}
