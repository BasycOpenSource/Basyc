using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Basyc.Diagnostics.Producing.Shared.Listening.SystemDiagnostics;

public class SystemDiagnosticsListener : IDiagnosticListener
{
    private ServiceIdentity service;

    public SystemDiagnosticsListener(IOptions<SystemDiagnosticsListenerOptions> options)
    {
        service = ServiceIdentity.ApplicationWideIdentity;
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
        var listener = new ActivityListener();
        listener.ShouldListenTo = activity =>
        {
            return true;
        };
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> options) =>
        {
            return ActivitySamplingResult.AllDataAndRecorded;
        };
        listener.ActivityStarted += (Activity activity) =>
        {
            if (options.Value.Filter.Invoke(activity) is false)
                return;

            string traceId = activity.TraceId.ToString();
            string? parentId = activity.ParentSpanId == default ? null : activity.ParentSpanId.ToString();
            var actvityStart = new ActivityStart(service, traceId, parentId, activity.SpanId.ToString(), activity.OperationName, activity.StartTimeUtc);
            ActivityStartsReceived?.Invoke(this, actvityStart);
        };
        listener.ActivityStopped += activity =>
        {
            if (options.Value.Filter.Invoke(activity) is false)
                return;
            string traceId = activity.TraceId.ToString();
            string? parentId = activity.ParentSpanId == default ? null : activity.ParentSpanId.ToString();
            var activityEnd = new ActivityEnd(service, traceId, parentId, activity.SpanId.ToString(), activity.OperationName, activity.StartTimeUtc, activity.StartTimeUtc + activity.Duration, activity.Status);
            ActivityEndsReceived?.Invoke(this, activityEnd);

        };
        ActivitySource.AddActivityListener(listener);
    }

    public event EventHandler<LogEntry>? LogsReceived;
    public event EventHandler<ActivityStart>? ActivityStartsReceived;
    public event EventHandler<ActivityEnd>? ActivityEndsReceived;

    public Task Start()
    {
        return Task.CompletedTask;
    }
}
