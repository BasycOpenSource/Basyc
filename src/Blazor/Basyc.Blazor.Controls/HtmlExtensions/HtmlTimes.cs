namespace Basyc.Blazor.Controls.HtmlExtensions;

public static partial class HtmlTimes
{
    public static string Time(this IHtmlMethods methods, TimeSpan duration) => $"{Math.Ceiling(duration.TotalMilliseconds)} ms";

    public static string Time(this IHtmlMethods methods, DateTime dateTime) => dateTime.ToString("HH:mm:ss:ffff");

    public static string Time(this IHtmlMethods methods, DateTimeOffset dateTimeOffset) => dateTimeOffset.LocalDateTime.ToString("HH:mm:ss:ffff");
}
