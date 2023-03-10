namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Html;

public static partial class Html
{
	public static string Time(TimeSpan duration)
	{
		return $"{Math.Ceiling(duration.TotalMilliseconds)} ms";
	}

	public static string Time(DateTime dateTime)
	{
		return dateTime.ToString("HH:mm:ss:ffff");
	}

	public static string Time(DateTimeOffset dateTimeOffset)
	{
		return dateTimeOffset.LocalDateTime.ToString("HH:mm:ss:ffff");
	}
}
