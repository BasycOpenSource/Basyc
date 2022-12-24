using Basyc.Diagnostics.Shared.Durations;
using Microsoft.Extensions.Logging;

namespace Basyc.Diagnostics.Shared.Logging;

public record struct LogEntry(ServiceIdentity Service, string TraceId, DateTimeOffset Time, LogLevel LogLevel, string Message, string? SpanId);
