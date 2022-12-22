using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;

public record class ActivityStartsReceivedArgs(ActivityStart[] ActivityStarts);
