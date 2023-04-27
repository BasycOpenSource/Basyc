namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Htmls;

public static partial class Html
{
    public static string Time(TimeSpan duration) => $"{Math.Ceiling(duration.TotalMilliseconds)} ms";

    public static string Time(DateTime dateTime) => dateTime.ToString("HH:mm:ss:ffff");

    public static string Time(DateTimeOffset dateTimeOffset) => dateTimeOffset.LocalDateTime.ToString("HH:mm:ss:ffff");
}
