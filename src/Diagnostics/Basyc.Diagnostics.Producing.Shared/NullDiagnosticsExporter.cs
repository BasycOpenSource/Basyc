using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Producing.Shared;

/// <summary>
/// Dummy producer implementation that does nothing
/// </summary>
public class NullDiagnosticsExporter : IDiagnosticsExporter
{
	public Task EndActivity(ActivityEnd activity)
	{
		return Task.CompletedTask;
	}

	public Task ProduceLog(LogEntry logEntry)
	{
		return Task.CompletedTask;
	}

	public Task StartActivity(ActivityStart activity)
	{
		return Task.CompletedTask;
	}

	public Task<bool> StartAsync()
	{
		return Task.FromResult(true);
	}
}
