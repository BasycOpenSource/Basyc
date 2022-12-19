using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Server.Abstractions
{
	public class InMemoryServerDiagnosticReceiver : IServerDiagnosticReceiver
	{
		public event EventHandler<DiagnosticChanges>? ChangesReceived;
		public InMemoryServerDiagnosticReceiver()
		{
		}

		public void ReceiveChangesFromProducer(DiagnosticChanges changes)
		{
			ChangesReceived?.Invoke(this, changes);
		}
	}
}
