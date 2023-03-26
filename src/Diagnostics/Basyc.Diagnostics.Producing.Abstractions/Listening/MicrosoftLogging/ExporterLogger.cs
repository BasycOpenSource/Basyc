using Basyc.Diagnostics.Shared;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Basyc.Diagnostics.Producing.Shared.Listening.MicrosoftLogging;

internal class ExporterLogger : ILogger
{

	private class NullScope : IDisposable
	{
		public static IDisposable Instance { get; } = new NullScope();

		public void Dispose()
		{
		}
	}

	private readonly MicrosoftLoggingDiagnosticListener listener;
	public ExporterLogger(MicrosoftLoggingDiagnosticListener listener)
	{
		this.listener = listener;
	}

	public IDisposable BeginScope<TState>(TState state)
	{
		return NullScope.Instance;
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return true;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (Activity.Current is null)
			return;

		var message = formatter.Invoke(state, exception);
		var traceId = Activity.Current.TraceId.ToString();
		var spanId = Activity.Current.SpanId.ToString();
		var logEntry = new Diagnostics.Shared.Logging.LogEntry(ServiceIdentity.ApplicationWideIdentity, traceId, DateTimeOffset.UtcNow, logLevel, message, spanId);
		listener.ReceiveLog(logEntry);
	}
}
