using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Basyc.MessageBus.Client.Diagnostics;

public class DummyLoggerCategory
{
}

public class BusHandlerLogger : ILogger
{
	private readonly IOptions<BusDiagnosticsOptions> busDiagnosticOptions;
	private readonly IDiagnosticsExporter[] logSinks;
	private readonly ILogger normalLogger;

	public BusHandlerLogger(ILogger normalLogger, IEnumerable<IDiagnosticsExporter> logSinks, IOptions<BusDiagnosticsOptions> busDiagnosticOptions)
	{
		this.normalLogger = normalLogger;
		this.busDiagnosticOptions = busDiagnosticOptions;
		this.logSinks = logSinks.ToArray();
	}

	public IDisposable BeginScope<TState>(TState state)
	{
		var normalLoggerNewScope = normalLogger.BeginScope(state);
		return normalLoggerNewScope;
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return true;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (BusHandlerLoggerSessionManager.HasSession(out var session) is false)
			throw new InvalidOperationException(
				$"Can't log without starting {nameof(BusHandlerLoggerSessionManager.StartSession)}. This logger should be only used for bus handlers");

		if (normalLogger.IsEnabled(logLevel))
			normalLogger.Log(logLevel, eventId, state, exception, formatter);

		foreach (var logSink in logSinks)
		{
			var message = formatter.Invoke(state, exception);
			var spanId = Activity.Current?.SpanId.ToString();
			var logEntry = new LogEntry(busDiagnosticOptions.Value.Service, session.TraceId, DateTimeOffset.UtcNow, logLevel, message, spanId);
			logSink.ProduceLog(logEntry);
		}
	}
}

public class BusHandlerLogger<THandler> : BusHandlerLogger, ILogger<THandler>
{
	public BusHandlerLogger(ILogger<THandler> normalLogger, IEnumerable<IDiagnosticsExporter> logSinks, IOptions<BusDiagnosticsOptions> busDiagnosticOptions)
		: base(normalLogger, logSinks, busDiagnosticOptions)
	{
	}
}
