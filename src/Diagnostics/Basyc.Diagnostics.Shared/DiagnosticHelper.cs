using System.Diagnostics;

namespace Basyc.Diagnostics.Shared
{
	/// <summary>
	/// Helper around standard <see cref="Activity"/>
	/// </summary>
	public static class DiagnosticHelper
	{
		/// <summary>
		/// No logic for this activity source. This frees you from create your own sources in your classes.
		/// </summary>
		public static readonly ActivitySource DefaultActivitySource = new ActivitySource(nameof(DefaultActivitySource), "1.0.0");
		public static DiagnosticHelperActivityDisposer Start(string name, string? traceId = null, string? parentSpanId = null)
		{
			if (traceId is null)
			{
				if (Activity.Current is not null)
					traceId = Activity.Current.TraceId.ToString();
				else
				{
					//throw new InvalidOperationException("Cant determine trace id because trace id is not set and parent actvitiy is not present");
					var activity2 = DiagnosticHelper.DefaultActivitySource.StartActivity(name);
					return new DiagnosticHelperActivityDisposer(activity2);
				}
			}
			var activityTraceId = ActivityTraceId.CreateFromString(traceId);

			ActivitySpanId parentActivitySpanId;
			if (parentSpanId is null)
			{
				if (Activity.Current is not null && Activity.Current.Id is not null)
					parentActivitySpanId = Activity.Current.SpanId;
				else
					parentActivitySpanId = default;
			}
			else
				parentActivitySpanId = ActivitySpanId.CreateFromString(parentSpanId);

			ActivityContext activityContext = new ActivityContext(activityTraceId, parentActivitySpanId, ActivityTraceFlags.Recorded);
			var activity = DiagnosticHelper.DefaultActivitySource.StartActivity(ActivityKind.Internal, name: name, parentContext: activityContext)!;
			return new DiagnosticHelperActivityDisposer(activity);

		}
	}
}
