using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

#pragma warning disable CA1819 // Properties should not return arrays
public record class LogsUpdatedArgs(LogEntry[] NewLogEntries);
