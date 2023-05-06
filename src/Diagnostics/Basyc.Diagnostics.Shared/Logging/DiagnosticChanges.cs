namespace Basyc.Diagnostics.Shared.Logging;

#pragma warning disable CA1819 // Properties should not return arrays
public record struct DiagnosticChanges(LogEntry[] Logs, ActivityStart[] ActivityStarts, ActivityEnd[] ActivityEnds);
