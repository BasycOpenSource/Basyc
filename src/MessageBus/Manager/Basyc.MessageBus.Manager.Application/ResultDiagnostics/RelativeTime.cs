namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;
public readonly struct RelativeTime
{
	public RelativeTime(DateTimeOffset time, DateTimeOffset baseTime)
	{
		BaseTime = baseTime;
		AbsoluteTime = time;
		Value = GetRelativeTime(time, baseTime);
	}

	public DateTimeOffset Value { get; init; }
	public DateTimeOffset AbsoluteTime { get; init; }
	public DateTimeOffset BaseTime { get; init; }

	private DateTimeOffset GetRelativeTime(DateTimeOffset toConvertTime, DateTimeOffset baseTime)
	{
		var relativeTime = new DateTimeOffset((toConvertTime - baseTime).Ticks, default);
		return relativeTime;
	}
}
