namespace Basyc.Diagnostics.Receiving.Abstractions;

public interface IDiagnosticReceiver
{
    event EventHandler<LogsReceivedArgs> LogsReceived;

    event EventHandler<ActivityStartsReceivedArgs> ActivityStartsReceived;

    event EventHandler<ActivityEndsReceivedArgs> ActivityEndsReceived;

    Task StartReceiving();
}
