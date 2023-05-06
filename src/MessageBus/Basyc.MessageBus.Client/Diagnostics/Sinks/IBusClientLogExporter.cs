using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Client.Diagnostics.Sinks;

public interface IBusClientLogExporter
{
    public void SendLog<TState>(string handlerDisplayName, LogLevel logLevel, string traceId, TState state, Exception exception, Func<TState, Exception, string> formatter);
}
