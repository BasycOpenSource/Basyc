using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Client.NetMQ.Sessions
{
	public class InMemorySessionManager<TSessionResult> : ISessionManager<TSessionResult>
	{
		public InMemorySessionManager(ILogger<InMemorySessionManager<TSessionResult>> logger)
		{
			this.logger = logger;
		}
		private int lastUsedSessionId = 0;
		private readonly Dictionary<int, NetMqSession<TSessionResult>> sessions = new Dictionary<int, NetMqSession<TSessionResult>>();
		private readonly ILogger<InMemorySessionManager<TSessionResult>> logger;

		/// <summary>
		/// Return new session's id
		/// </summary>
		/// <returns></returns>
		public NetMqSession<TSessionResult> CreateSession(string messageType, string? traceId)
		{
			var newSessionId = Interlocked.Increment(ref lastUsedSessionId);
			TaskCompletionSource<TSessionResult> responseSource = new TaskCompletionSource<TSessionResult>();
			var newSession = new NetMqSession<TSessionResult>(newSessionId, traceId, messageType, responseSource);
			sessions.Add(newSessionId, newSession);
			logger.LogDebug($"Session '{newSession.SessionId}' created for '{messageType}'");
			return newSession;
		}

		/// <summary>
		/// Returns true if session with specific id exists
		/// </summary>
		/// <param name="sessionId"></param>
		/// <returns></returns>
		public bool TryCompleteSession(int sessionId, TSessionResult sessionResult)
		{
			if (sessions.TryGetValue(sessionId, out var sessionToComplete) is false)
			{
				return false;
			}
			sessionToComplete.ResponseSource.SetResult(sessionResult);
			sessions.Remove(sessionId);
			logger.LogDebug($"Session '{sessionToComplete.SessionId}' completed for '{sessionToComplete.MessageType}'");
			return true;
		}

	}
}
