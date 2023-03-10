using System.Diagnostics;

namespace Basyc.Diagnostics.Shared;

public struct DiagnosticHelperActivityDisposer : IDisposable
{
	public DiagnosticHelperActivityDisposer(Activity? activity)
	{
		Activity = activity;
	}

	public Activity? Activity { get; }

	public void Stop()
	{
		Stop(DateTimeOffset.UtcNow);
	}

	public void Stop(DateTimeOffset endTime)
	{
		if (Activity is not null)
		{
			Activity.SetEndTime(endTime.UtcDateTime);
			Activity.Stop();
		}
	}

	public void Dispose()
	{
		Stop();
	}
}
