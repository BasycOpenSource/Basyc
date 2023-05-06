using System.Diagnostics;

namespace Basyc.Diagnostics.Shared;

#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct ActivityDisposer : IDisposable
{
    public ActivityDisposer(Activity? activity)
    {
        Activity = activity;
    }

    public Activity? Activity { get; }

    public DateTimeOffset Stop()
    {
        var endTime = DateTimeOffset.UtcNow;
        Stop(endTime);
        return endTime;
    }

    public void Stop(DateTimeOffset endTime)
    {
        if (Activity is not null)
        {
            Activity.SetEndTime(endTime.UtcDateTime);
            Activity.Stop();
        }
    }

    public void Dispose() => Stop();
}
