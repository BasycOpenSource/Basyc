using Microsoft.Extensions.Logging;

namespace Basyc.Diagnostics.Shared.Durations;

public interface IDurationMapBuilder : IDisposable
{
    DateTimeOffset EndTime { get; }

    bool HasStarted { get; }

    bool HasEnded { get; }

    ServiceIdentity Service { get; }

    DateTimeOffset StartTime { get; }

    void End();

    void End(DateTimeOffset endTime);

    DateTimeOffset Start();

    IDurationSegmentBuilder StartNewSegment(ServiceIdentity service, string segmentName, DateTimeOffset startTime);

    IDurationSegmentBuilder StartNewSegment(string segmentName);

    IDurationSegmentBuilder StartNewSegment(string segmentName, DateTimeOffset startTime);

    ValueTask Log(string message, LogLevel logLevel);
}
