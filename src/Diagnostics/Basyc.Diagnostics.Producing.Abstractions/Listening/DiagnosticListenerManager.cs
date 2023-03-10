using Basyc.Diagnostics.Producing.Abstractions;
using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Producing.Shared.Listening;

public class DiagnosticListenerManager
{
	private readonly IDiagnosticsExporter[] exporters;
	private readonly IDiagnosticListener[] listeners;

	public DiagnosticListenerManager(IEnumerable<IDiagnosticsExporter> exporters, IEnumerable<IDiagnosticListener> listeners)
	{
		this.exporters = exporters.ToArray();
		this.listeners = listeners.ToArray();
	}

	public async Task Start()
	{
		foreach (var listener in listeners)
		{
			listener.LogsReceived += Listener_LogsReceived;
			listener.ActivityStartsReceived += Listener_ActivityStartsReceived;
			listener.ActivityEndsReceived += Listener_ActivityEndsReceived;
			await listener.Start();
		}
	}

	private void Listener_ActivityEndsReceived(object? sender, ActivityEnd e)
	{
		foreach (var exporter in exporters)
			exporter.EndActivity(e);
	}

	private void Listener_ActivityStartsReceived(object? sender, ActivityStart e)
	{
		foreach (var exporter in exporters)
			exporter.StartActivity(e);
	}

	private void Listener_LogsReceived(object? sender, LogEntry e)
	{
		foreach (var exporter in exporters)
			exporter.ProduceLog(e);
	}
}
