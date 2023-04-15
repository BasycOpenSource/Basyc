﻿using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class ResultLoggingContextLogger : ILogger
{
	private readonly MessageDiagnostic loggingContext;
	private readonly ServiceIdentity serviceIdentity;

	public ResultLoggingContextLogger(ServiceIdentity serviceIdentity, MessageDiagnostic loggingContext)
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
		loggingContext.AddLog(serviceIdentity, logLevel, message, spanId);
	}
}
