namespace Basyc.Diagnostics.Receiving.Abstractions;

#pragma warning disable CA1003 // Use generic event handler instances

public interface IDiagnosticReceiver
{
    event EventHandler<LogsReceivedArgs> LogsReceived;

    event EventHandler<ActivityStartsReceivedArgs> ActivityStartsReceived;

    event EventHandler<ActivityEndsReceivedArgs> ActivityEndsReceived;

    Task StartReceiving();
}
