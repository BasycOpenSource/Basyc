using System.Diagnostics;

namespace Basyc.Diagnostics.Shared;

/// <summary>
/// Helper around standard <see cref="Activity"/>
/// </summary>
public static class DiagnosticHelper
{
	/// <summary>
	/// No logic for this activity source. This frees you from create your own sources in your classes.
	/// </summary>
	public static readonly ActivitySource DefaultActivitySource = new(nameof(DefaultActivitySource), "1.0.0");
	public static ActivityDisposer Start(string name, string? traceId = null, string? parentSpanId = null)
	{
		if (traceId is null)
		{
			if (Activity.Current is not null)
				traceId = Activity.Current.TraceId.ToString();
			else
			{
				var newActivityWithoutParent = DiagnosticHelper.DefaultActivitySource.StartActivity(name);
				return new ActivityDisposer(newActivityWithoutParent);
			}
		}

		var activityTraceId = ActivityTraceId.CreateFromString(traceId);
		var parentActivitySpanId = parentSpanId is null
			? Activity.Current is not null && Activity.Current.Id is not null ? Activity.Current.SpanId : default
			: ActivitySpanId.CreateFromString(parentSpanId);
		var activityContext = new ActivityContext(activityTraceId, parentActivitySpanId, ActivityTraceFlags.Recorded);
		var activity = DiagnosticHelper.DefaultActivitySource.StartActivity(ActivityKind.Internal, name: name, parentContext: activityContext)!;
		return new ActivityDisposer(activity);

	}
}
