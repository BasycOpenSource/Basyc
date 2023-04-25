using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Client.NetMQ.Sessions;

public class InMemorySessionManager<TSessionResult> : ISessionManager<TSessionResult>
{
    private readonly Dictionary<int, NetMqSession<TSessionResult>> sessions = new();
    private readonly ILogger<InMemorySessionManager<TSessionResult>> logger;
    private int lastUsedSessionId;

    public InMemorySessionManager(ILogger<InMemorySessionManager<TSessionResult>> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Return new session's id.
    /// </summary>
    public NetMqSession<TSessionResult> CreateSession(string messageType, string? traceId)
    {
        var newSessionId = Interlocked.Increment(ref lastUsedSessionId);
        TaskCompletionSource<TSessionResult> responseSource = new TaskCompletionSource<TSessionResult>();
        var newSession = new NetMqSession<TSessionResult>(newSessionId, traceId, messageType, responseSource);
        sessions.Add(newSessionId, newSession);
        logger.LogDebug("Session '{SessionId}' created for '{Type}'", newSession.SessionId, messageType);
        return newSession;
    }

    /// <summary>
    /// Returns true if session with specific id exists.
    /// </summary>
    public bool TryCompleteSession(int sessionId, TSessionResult sessionResult)
    {
        if (sessions.TryGetValue(sessionId, out var sessionToComplete) is false)
        {
            return false;
        }

        sessionToComplete.ResponseSource.SetResult(sessionResult);
        sessions.Remove(sessionId);
        logger.LogDebug("Session '{SessionId}' completed for '{Type}'", sessionToComplete.SessionId, sessionToComplete.MessageType);
        return true;
    }
}
