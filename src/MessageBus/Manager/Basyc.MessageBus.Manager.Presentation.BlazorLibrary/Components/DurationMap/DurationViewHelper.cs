using System.Globalization;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.DurationMap;

public static class DurationViewHelper
{
	private readonly static NumberFormatInfo numberFormatter;
	static DurationViewHelper()
	{
		numberFormatter = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
		numberFormatter.NumberDecimalSeparator = ".";
	}

	public static string GetCssDurationValue(TimeSpan duration, double scale, out double lenghtNumber, string unit = "px")
	{
		var displayLenght = Math.Round(duration.TotalMilliseconds) * scale;
		lenghtNumber = displayLenght;
		return $"{displayLenght.ToString(numberFormatter)}{unit}";
	}

	public static string GetCssDurationValue(TimeSpan duration, double scale, string unit = "px")
	{
		return GetCssDurationValue(duration, scale, out _, unit);
	}

	public static string GetCssDurationValue(DateTimeOffset earlyTime, DateTimeOffset laterTime, double scale, out double remNumber, string unit = "px")
	{
		return GetCssDurationValue(laterTime - earlyTime, scale, out remNumber, unit);
	}

	public static string GetCssDurationValue(DateTimeOffset earlyTime, DateTimeOffset laterTime, double scale, string unit = "px")
	{
		return GetCssDurationValue(earlyTime, laterTime, scale, out _, unit);
	}
}
