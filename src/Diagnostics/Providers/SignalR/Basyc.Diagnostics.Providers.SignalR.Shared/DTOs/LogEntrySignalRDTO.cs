using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace Basyc.Diagnostics.SignalR.Shared.DTOs
{
    public record LogEntrySignalRDTO(ServiceIdentity Service, string TraceId, DateTimeOffset Time, LogLevel LogLevel, string Message, string? SpanId)
    {
        public static LogEntrySignalRDTO FromEntry(LogEntry logEntry)
        {
            return new LogEntrySignalRDTO(logEntry.Service, logEntry.TraceId, logEntry.Time, logEntry.LogLevel, logEntry.Message, logEntry.SpanId);
        }

        public static LogEntry ToEntry(LogEntrySignalRDTO logEntryDTO)
        {
            return new LogEntry(logEntryDTO.Service, logEntryDTO.TraceId, logEntryDTO.Time, logEntryDTO.LogLevel, logEntryDTO.Message, logEntryDTO.SpanId);
        }
    }



}