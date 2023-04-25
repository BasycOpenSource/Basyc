using System;
using System.Threading;

namespace Basyc.MessageBus.Client.Diagnostics;

public static class BusHandlerLoggerSessionManager
{
    private static AsyncLocal<LoggingSession> SessionId { get; } = new AsyncLocal<LoggingSession>();

    private static AsyncLocal<bool> HasSesion { get; } = new AsyncLocal<bool>() { Value = false };

    public static void StartSession(LoggingSession loggingSession)
    {
        if (HasSesion.Value is true)
        {
            throw new InvalidOperationException($"Cant call {nameof(StartSession)} twice on same async context");
        }

        SessionId.Value = loggingSession;
        HasSesion.Value = true;
    }

    public static void EndSession()
    {
        if (HasSesion.Value is false)
        {
            throw new InvalidOperationException($"Cant call {nameof(EndSession)} without calling {nameof(StartSession)} before");
        }

        SessionId.Value = default;
        HasSesion.Value = false;
    }

    public static bool HasSession(out LoggingSession loggingSession)
    {
        loggingSession = SessionId.Value;
        return HasSesion.Value;
    }
}
