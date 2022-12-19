namespace Basyc.MessageBus.Client.Diagnostics
{
	public readonly record struct LoggingSession(string TraceId, string HandlerName);
}
