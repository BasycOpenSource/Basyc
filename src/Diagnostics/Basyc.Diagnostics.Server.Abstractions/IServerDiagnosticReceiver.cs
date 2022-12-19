using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Server.Abstractions
{
	public interface IServerDiagnosticReceiver
	{
		event EventHandler<DiagnosticChanges> ChangesReceived;
	}
}