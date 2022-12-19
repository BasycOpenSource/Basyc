using Basyc.Diagnostics.Shared.Durations;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Basyc.MessageBus.Manager.Application.Requesting
{
	public class ResultLoggingContextLogger : ILogger
	{
		private readonly RequestDiagnosticContext loggingContext;
		private readonly ServiceIdentity serviceIdentity;

		public ResultLoggingContextLogger(ServiceIdentity serviceIdentity, RequestDiagnosticContext loggingContext)
		{
			this.loggingContext = loggingContext;
			this.serviceIdentity = serviceIdentity;
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
			var spanId = Activity.Current?.SpanId.ToString();
			var message = formatter.Invoke(state, exception);
			loggingContext.Log(serviceIdentity, logLevel, message, spanId);
		}
	}
}
