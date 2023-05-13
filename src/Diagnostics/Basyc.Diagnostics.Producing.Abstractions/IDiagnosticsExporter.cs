using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Helpers;
using Basyc.Diagnostics.Shared.Logging;
using System.Diagnostics;

namespace Basyc.Diagnostics.Producing.Abstractions;

public interface IDiagnosticsExporter
{
    Task ProduceLog(LogEntry logEntry);

    Task StartActivity(ActivityStart activityStart);

    Task EndActivity(ActivityEnd activityEnd);

    Task<bool> StartAsync();

    public ActivityStartDisposer StartActivity(ServiceIdentity serviceIdentity, string traceId, string? parentId, string name, DateTimeOffset startTime = default)
    {
        if (startTime == default)
            startTime = DateTimeOffset.UtcNow;
        var activityStart = new ActivityStart(serviceIdentity, traceId, parentId, IdGeneratorHelper.GenerateNewSpanId(), name, startTime);
        var disposer = new ActivityStartDisposer(this, activityStart);
        StartActivity(activityStart);
        return disposer;
    }

    public ActivityStartDisposer StartActivity(string traceId, string? parentId, string name, DateTimeOffset startTime = default) => StartActivity(ServiceIdentity.ApplicationWideIdentity, traceId, parentId, name, startTime);

    public ActivityStartDisposer StartActivity(ActivityStart parentActivityStart, string name, DateTimeOffset startTime = default) => StartActivity(ServiceIdentity.ApplicationWideIdentity, parentActivityStart.TraceId, parentActivityStart.Id, name, startTime);

    public ActivityStartDisposer StartActivity(ActivityStartDisposer parentActivityStart, string name, DateTimeOffset startTime = default) => StartActivity(parentActivityStart.ActivityStart, name, startTime);

    public Task EndActivity(ActivityStart activity, DateTimeOffset endtime = default, ActivityStatusCode status = ActivityStatusCode.Ok)
    {
        if (endtime == default)
            endtime = DateTimeOffset.UtcNow;
        return EndActivity(new ActivityEnd(activity.Service, activity.TraceId, activity.ParentId, activity.Id, activity.Name, activity.StartTime, endtime, status));
    }
}
