namespace Basyc.Diagnostics.Shared.Durations
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="Segments"> </param>
	/// <param name="TotalDuration"> Indicates total duration (combining sending request, processing request and waiting time for a response)</param>
	public record DurationMap(DurationSegment[] Segments, TimeSpan TotalDuration, DateTimeOffset StartTime, DateTimeOffset EndTime);
}