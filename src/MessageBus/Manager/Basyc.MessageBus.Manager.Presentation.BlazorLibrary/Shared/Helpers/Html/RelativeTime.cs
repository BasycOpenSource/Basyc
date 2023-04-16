using Basyc.MessageBus.Manager.Application.ResultDiagnostics;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Helpers.Html;
public static partial class Html
{
	public static string RelativeTime(this RelativeTime relativeTime)
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
