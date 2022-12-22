using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Helpers;
using Basyc.Diagnostics.Shared.Logging;
using System.Diagnostics;

namespace Basyc.Diagnostics.Producing.Shared;

public interface IDiagnosticsExporter
{
	Task ProduceLog(LogEntry logEntry);
	Task StartActivity(ActivityStart activityStart);
	Task EndActivity(ActivityEnd activityEnd);
	Task<bool> StartAsync();

	/// <summary>
	/// When app represents only one identity (in most sc)
	/// </summary>
	public ActivityDisposer StartActivity(ServiceIdentity serviceIdentity, string traceId, string? parentId, string name, DateTimeOffset startTime = default)
	{
		if (startTime == default)
			startTime = DateTimeOffset.UtcNow;
		var activityStart = new ActivityStart(serviceIdentity, traceId, parentId, IdGeneratorHelper.GenerateNewSpanId(), name, startTime);
		var disposer = new ActivityDisposer(this, activityStart);
		this.StartActivity(activityStart);
		return disposer;
	}

	public ActivityDisposer StartActivity(string traceId, string? parentId, string name, DateTimeOffset startTime = default)
	{

		return StartActivity(ServiceIdentity.ApplicationWideIdentity, traceId, parentId, name, startTime);
	}

	public ActivityDisposer StartActivity(ActivityStart parentActivityStart, string name, DateTimeOffset startTime = default)
	{
		return StartActivity(ServiceIdentity.ApplicationWideIdentity, parentActivityStart.TraceId, parentActivityStart.Id, name, startTime);
	}

	public ActivityDisposer StartActivity(ActivityDisposer parentActivityStart, string name, DateTimeOffset startTime = default)
	{
		return StartActivity(parentActivityStart.ActivityStart, name, startTime);
	}

	public Task EndActivity(ActivityStart activity, DateTimeOffset endtime = default, ActivityStatusCode status = ActivityStatusCode.Ok)
	{
		if (endtime == default)
			endtime = DateTimeOffset.UtcNow;
		return EndActivity(new ActivityEnd(activity.Service, activity.TraceId, activity.ParentId, activity.Id, activity.Name, activity.StartTime, endtime, status));
	}
}