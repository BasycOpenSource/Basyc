namespace Basyc.Diagnostics.Shared.Logging
{
	public record struct DiagnosticChanges(LogEntry[] Logs, ActivityStart[] ActivityStarts, ActivityEnd[] ActivityEnds);
}
