using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Receiving.Abstractions;

public record LogsReceivedArgs(LogEntry[] LogEntries);
