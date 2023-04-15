namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics;
public static class DateTimeOffsetRelativeTimeExntesions
{
	public static RelativeTime GetRelativeTime(this DateTimeOffset toConvertTime, DateTimeOffset baseTime)
	{
		return new RelativeTime(toConvertTime, baseTime);
	}
}
