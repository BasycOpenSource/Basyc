namespace Basyc.Diagnostics.Shared.Durations
{
	public record DurationSegment(ServiceIdentity Service, string Name, DateTimeOffset StartTime, DateTimeOffset EndTime, TimeSpan Duration, DurationSegment[] NestedSegments);
}
