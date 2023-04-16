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
		var relativeTicks = (toConvertTime - baseTime).Ticks;
		if (relativeTicks < 0)
		{
			relativeTicks = 0;
		}
		try
		{
			var relativeTime = new DateTimeOffset(relativeTicks, default);
			return relativeTime;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}
	}
}
