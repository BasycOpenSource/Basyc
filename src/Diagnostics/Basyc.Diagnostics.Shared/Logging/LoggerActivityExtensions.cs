using Basyc.Diagnostics.Shared;

namespace Microsoft.Extensions.Logging;
#pragma warning disable IDE0060

public static class LoggerActivityExtensions
{
    /// <summary>
    ///     Creates new activity with new trace.
    /// </summary>
    public static ActivityDisposer StartActivity(this ILogger logger, string name) => StartActivity(logger, name, null);

    public static ActivityDisposer StartActivity(this ILogger logger, string name, string? traceId = null, string? parentSpanId = null) =>
        DiagnosticHelper.Start(name, traceId, parentSpanId);
}
