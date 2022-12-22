using System.Diagnostics;

namespace Basyc.Diagnostics.Producing.Shared.Listening.SystemDiagnostics;

public class SystemDiagnosticsListenerOptions
{
	public Func<Activity, bool> Filter { get; set; } = (a) => true;
}