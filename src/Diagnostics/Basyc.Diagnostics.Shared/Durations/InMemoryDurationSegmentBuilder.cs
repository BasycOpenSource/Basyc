using Basyc.Diagnostics.Shared.Helpers;
using Microsoft.Extensions.Logging;

namespace Basyc.Diagnostics.Shared.Durations;

public class InMemoryDurationSegmentBuilder : DurationSegmentBuilderBase, IDisposable, IDurationSegmentBuilder
{
	private readonly List<InMemoryDurationSegmentBuilder> nestedSegmentBuilders = new List<InMemoryDurationSegmentBuilder>();
	private readonly Func<InMemoryDurationSegmentBuilder>? parentSegmentGetter;

	public InMemoryDurationSegmentBuilder(ServiceIdentity service, string traceId, string id, string name) : base(service, traceId, id, name)
	{
		Name = name;
		Service = service;
	}

	public InMemoryDurationSegmentBuilder(ServiceIdentity service, string traceId, string id, string name, Func<InMemoryDurationSegmentBuilder> parentSegmentGetter)
		: this(service, name, traceId, id)
	{
		this.parentSegmentGetter = parentSegmentGetter;
		HasParent = true;
	}

	public InMemoryDurationSegmentBuilder(ServiceIdentity service, string traceId, string id, string name, DateTimeOffset segmentStart, InMemoryDurationSegmentBuilder parentSegment)
		: this(service, traceId, id, name, () => parentSegment)
	{
		StartTime = segmentStart;
		HasStarted = true;
	}

	/// <summary>
	/// Ensures that <see cref="End"/> was called and produce <see cref="DurationSegment"/> with processed timestamps
	/// </summary>
	public DurationSegment Build()
	{
		var wasStarted = EnsureStarted(out var starTtime);

		if (HasEnded is false)
		{
			End();
		}

		return Build(EndTime);
	}

	/// <summary>
	/// Ensures that <see cref="End"/> was called and produce <see cref="DurationSegment"/> with processed timestamps
	/// </summary>
	public DurationSegment Build(DateTimeOffset finalEndTime)
	{
		if (EnsureStarted(finalEndTime) is false)
		{
			if (finalEndTime < StartTime)
			{
				throw new ArgumentException("Final end time can't be sooner than actual startTime time");
			}
		}

		if (HasEnded is false)
		{
			End(finalEndTime);
		}
		else
		{
			if (finalEndTime < EndTime)
			{
				throw new ArgumentException("Final end time can't be sooner than actual end time");
			}
		}

		return BuildNestedSegments();
	}

	private DurationSegment BuildNestedSegments()
	{
		DurationSegment[] nestedSegments = new DurationSegment[nestedSegmentBuilders.Count];
		for (int nestedSegmentIndex = 0; nestedSegmentIndex < nestedSegmentBuilders.Count; nestedSegmentIndex++)
		{
			InMemoryDurationSegmentBuilder? nestedSegmentBuilder = nestedSegmentBuilders[nestedSegmentIndex];
			var nestedSegment = nestedSegmentBuilder.Build(EndTime);
			nestedSegments[nestedSegmentIndex] = nestedSegment;
		}

		return new DurationSegment(Service, Name, StartTime, EndTime, EndTime - StartTime, nestedSegments);
	}
	public override void End(DateTimeOffset finalEndTime)
	{
		if (HasEnded)
		{
			throw new InvalidOperationException($"{nameof(End)} was called twice");
		}

		EndTime = finalEndTime;

		foreach (var nestedSegment in nestedSegmentBuilders)
		{
			if (nestedSegment.HasEnded is false)
			{
				nestedSegment.End(finalEndTime);
			}
		}

		HasEnded = true;
	}

	public override IDurationSegmentBuilder EndAndStartFollowing(string segmentName)
	{
		var endTime = DateTimeOffset.UtcNow;
		End(endTime);
		if (HasParent is false)
		{
			throw new InvalidOperationException("Cannot create following segment because this segment deos not have a parent");
		}

		return parentSegmentGetter!.Invoke().StartNested(segmentName, endTime);
	}

	public override IDurationSegmentBuilder StartNested(ServiceIdentity service, string segmentName, DateTimeOffset start)
	{
		EnsureStarted(start);
		var nestedSegment = new InMemoryDurationSegmentBuilder(service, TraceId, IdGeneratorHelper.GenerateNewSpanId(), segmentName, start, this);
		nestedSegmentBuilders.Add(nestedSegment);
		return nestedSegment;
	}

	public override ValueTask Log(string message, LogLevel logLevel)
	{
		throw new NotImplementedException();
	}
}
