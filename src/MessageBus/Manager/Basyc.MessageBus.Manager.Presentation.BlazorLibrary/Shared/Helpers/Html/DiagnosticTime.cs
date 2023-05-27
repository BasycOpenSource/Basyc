using Basyc.MessageBus.Manager.Application.ResultDiagnostics;

namespace Basyc.Blazor.Controls.HtmlExtensions;

public static partial class HtmlDiagnosticTimes
{
    public static string DiagnosticTime(this IHtmlMethods methods, DiagnosticTime relativeTime)
    {
        //var totalMs = relativeTime.Value.ToUnixTimeMilliseconds();
        var totalMs = relativeTime.Value.Ticks / (double)TimeSpan.TicksPerMillisecond;
        var grade = totalMs switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
        return $"{totalMs}{grade} ms";
    }
}
