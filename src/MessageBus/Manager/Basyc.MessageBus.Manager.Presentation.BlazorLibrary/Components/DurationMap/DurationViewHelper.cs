using Basyc.Blazor.Controls.HtmlExtensions;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Components.DurationMap;

public static class DurationViewHelper
{
    public static string GetCssDurationValue(TimeSpan duration, double scale, out double lenghtNumber, string unit = "px")
    {
        lenghtNumber = Math.Round(duration.TotalMilliseconds) * scale;
        var lenghtNumberText = HtmlNumbers.Number(null, lenghtNumber);
        return $"{lenghtNumberText}{unit}";
    }

    public static string GetCssDurationValue(TimeSpan duration, double scale, string unit = "px") => GetCssDurationValue(duration, scale, out _, unit);

    public static string GetCssDurationValue(DateTimeOffset earlyTime, DateTimeOffset laterTime, double scale, out double remNumber, string unit = "px") => GetCssDurationValue(laterTime - earlyTime, scale, out remNumber, unit);

    public static string GetCssDurationValue(DateTimeOffset earlyTime, DateTimeOffset laterTime, double scale, string unit = "px") => GetCssDurationValue(earlyTime, laterTime, scale, out _, unit);
}
