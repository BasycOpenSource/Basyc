using Microsoft.Extensions.Logging;

namespace Basyc.Diagnostics.Shared.Durations;

public abstract class DurationSegmentBuilderBase : IDisposable, IDurationSegmentBuilder
{
	public bool HasParent { get; protected set; }

	public DateTimeOffset EndTime { get; protected set; }
	public DateTimeOffset StartTime { get; protected set; }
	public bool HasStarted { get; protected set; }
	public bool HasEnded { get; protected set; }
	public string Name { get; init; }
	public string Id { get; init; }
	public string TraceId { get; init; }
	/// <summary>
	/// Service (application) executing this segment
	/// </summary>
	public ServiceIdentity Service { get; init; }

	public DurationSegmentBuilderBase(ServiceIdentity service, string traceId, string id, string name)
	{
		Name = name;
		Service = service;
		TraceId = traceId;
		Id = id;
	}

	public virtual void Start(DateTimeOffset segmentStart)
	{
		if (HasStarted)
			throw new InvalidOperationException("Can't start twice!");
		StartTime = segmentStart;
		HasStarted = true;
	}

	public virtual DateTimeOffset Start()
	{
		StartTime = DateTimeOffset.UtcNow;
		HasStarted = true;
		return StartTime;
	}

	public virtual DateTimeOffset End()
	{
		End(DateTimeOffset.UtcNow);
		return EndTime;
	}

	public abstract void End(DateTimeOffset finalEndTime);

	/// <summary>
	/// End current segment and craetes following segment (Not nested segment). 
	/// This ensures that there is not a gap between end and start of the new following segment
	/// Call only when segment has a parent!
	/// </summary>
	/// <param name="segmentName"></param>
	/// <returns></returns>
	public abstract IDurationSegmentBuilder EndAndStartFollowing(string segmentName);

	public virtual void Dispose()
	{
		End();
	}

	/// <summary>
	/// Return false when root had to be started
	/// </summary>
	/// <param name="starTime"></param>
	/// <returns></returns>
	protected bool EnsureStarted(out DateTimeOffset starTime)
	{
		if (HasStarted)
		{
			starTime = StartTime;
			return true;
		}
		else
		{
			starTime = Start();
			return false;
		}
	}

	/// <summary>
	/// Return false when root had to be started
	/// </summary>
	protected bool EnsureStarted(DateTimeOffset start)
	{
		if (HasStarted is false)
		{
			Start(start);
			return false;
		}

		return true;
	}

	public abstract IDurationSegmentBuilder StartNested(ServiceIdentity service, string segmentName, DateTimeOffset start);
	public virtual IDurationSegmentBuilder StartNested(ServiceIdentity service, string segmentName)
	{
		var wasStarted = EnsureStarted(out var rootStartTime);
		return StartNested(service, segmentName, wasStarted ? DateTimeOffset.UtcNow : rootStartTime);
	}

	public virtual IDurationSegmentBuilder StartNested(string segmentName, DateTimeOffset start)
	{
		return StartNested(Service, segmentName, start);
	}

	public virtual IDurationSegmentBuilder StartNested(string segmentName)
	{
		return StartNested(Service, segmentName);
	}

	public abstract ValueTask Log(string message, LogLevel logLevel);
}
