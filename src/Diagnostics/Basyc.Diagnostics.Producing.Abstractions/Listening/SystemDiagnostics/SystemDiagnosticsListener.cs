using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Basyc.Diagnostics.Producing.Shared.Listening.SystemDiagnostics;
#pragma warning disable CS0067 // The event 'SystemDiagnosticsListener.LogsReceived' is never used

public class SystemDiagnosticsListener : IDiagnosticListener, IDisposable
{
    private readonly ServiceIdentity service;
    private readonly ActivityListener listener;

    public SystemDiagnosticsListener(IOptions<SystemDiagnosticsListenerOptions> options)
    {
        service = ServiceIdentity.ApplicationWideIdentity;
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
        listener = new ActivityListener();
        listener.ShouldListenTo = activity =>
        {
            return true;
        };
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> options) =>
        {
            return ActivitySamplingResult.AllDataAndRecorded;
        };
        listener.ActivityStarted += activity =>
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
            var activityEnd = new ActivityEnd(
                service,
                traceId,
                parentId,
                activity.SpanId.ToString(),
                activity.OperationName,
                activity.StartTimeUtc,
                activity.StartTimeUtc + activity.Duration,
                activity.Status);
            ActivityEndsReceived?.Invoke(this, activityEnd);
        };
        ActivitySource.AddActivityListener(listener);
    }

    public event EventHandler<LogEntry>? LogsReceived;

    public event EventHandler<ActivityStart>? ActivityStartsReceived;

    public event EventHandler<ActivityEnd>? ActivityEndsReceived;

    public void Dispose() => listener.Dispose();

    public Task Start() => Task.CompletedTask;
}
