using System;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public interface IRequestDiagnosticsSource
{
	event EventHandler<LogsUpdatedArgs> LogsReceived;
	event EventHandler<ActivityStartsReceivedArgs> ActivityStartsReceived;
	event EventHandler<ActivityEndsReceivedArgs> ActivityEndsReceived;
}