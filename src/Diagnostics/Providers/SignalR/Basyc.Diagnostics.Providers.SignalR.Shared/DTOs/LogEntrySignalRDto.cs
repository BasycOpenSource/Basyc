using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace Basyc.Diagnostics.SignalR.Shared.DTOs;

public record LogEntrySignalRDto(ServiceIdentity Service, string TraceId, DateTimeOffset Time, LogLevel LogLevel, string Message, string? SpanId)
{
    public static LogEntrySignalRDto FromEntry(LogEntry logEntry) => new(logEntry.Service, logEntry.TraceId, logEntry.Time, logEntry.LogLevel, logEntry.Message, logEntry.SpanId);

    public static LogEntry ToEntry(LogEntrySignalRDto logEntryDto) => new(logEntryDto.Service, logEntryDto.TraceId, logEntryDto.Time, logEntryDto.LogLevel, logEntryDto.Message, logEntryDto.SpanId);
}
