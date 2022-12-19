using Microsoft.Extensions.Logging;

namespace Basyc.Diagnostics.Shared.Durations
{
	public interface IDurationSegmentBuilder : IDisposable
	{
		string Name { get; init; }
		string Id { get; init; }
		string TraceId { get; init; }
		bool HasStarted { get; }
		bool HasEnded { get; }
		DateTimeOffset StartTime { get; }
		DateTimeOffset EndTime { get; }

		DateTimeOffset Start();
		void Start(DateTimeOffset segmentStart);
		DateTimeOffset End();
		void End(DateTimeOffset finalEndTime);

		IDurationSegmentBuilder StartNested(ServiceIdentity service, string segmentName, DateTimeOffset start);
		IDurationSegmentBuilder StartNested(string segmentName, DateTimeOffset start);
		IDurationSegmentBuilder StartNested(ServiceIdentity service, string segmentName);
		IDurationSegmentBuilder StartNested(string segmentName);
		IDurationSegmentBuilder EndAndStartFollowing(string segmentName);
		ValueTask Log(string message, LogLevel logLevel);

	}
}