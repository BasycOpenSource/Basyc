using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Server.Abstractions;

#pragma warning disable CA1003 // Use generic event handler instances

public interface IServerDiagnosticReceiver
{
    event EventHandler<DiagnosticChanges> ChangesReceived;
}
