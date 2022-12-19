using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Server.Abstractions
{
    public interface IServerDiagnosticPusher
    {
        Task PushChangesToReceivers(DiagnosticChanges changes);
    }
}