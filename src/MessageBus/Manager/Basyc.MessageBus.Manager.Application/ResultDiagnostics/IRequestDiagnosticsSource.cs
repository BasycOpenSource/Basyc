namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

#pragma warning disable CA1003 // Use generic event handler instances
public interface IRequestDiagnosticsSource
{
    event EventHandler<LogsUpdatedArgs> LogsReceived;

    event EventHandler<ActivityStartsReceivedArgs> ActivityStartsReceived;

    event EventHandler<ActivityEndsReceivedArgs> ActivityEndsReceived;
}
