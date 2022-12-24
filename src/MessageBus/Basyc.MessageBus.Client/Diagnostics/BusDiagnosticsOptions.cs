using Basyc.Diagnostics.Shared.Durations;

namespace Basyc.MessageBus.Client.Diagnostics;

public class BusDiagnosticsOptions
{
	public ServiceIdentity Service { get; set; }
	public bool UseDiagnostics { get; set; }
}
