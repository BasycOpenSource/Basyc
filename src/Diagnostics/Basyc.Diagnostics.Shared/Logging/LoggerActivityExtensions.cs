using Basyc.Diagnostics.Shared;

namespace Microsoft.Extensions.Logging;
public static class LoggerActivityExtensions
{
	/// <summary>
	/// Creates new activity with new trace
	/// </summary>
	/// <param name="logger"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public static ActivityDisposer StartActivity(this ILogger logger, string name)
	{
		return StartActivity(logger, name, null, null);
	}

	public static ActivityDisposer StartActivity(this ILogger logger, string name, string? traceId = null, string? parentSpanId = null)
	{
		return DiagnosticHelper.Start(name, traceId, parentSpanId);
	}
}
