using Basyc.Diagnostics.Shared;
using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Helpers;
using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics.Durations;

internal class InMemoryDiagnosticsSourceDurationMapBuilder : InMemoryDiagnosticsSourceDurationSegmentBuilder, IDurationMapBuilder
{
    private readonly InMemoryRequestDiagnosticsSource diagnosticsSource;
    private readonly string? parentId;

    public InMemoryDiagnosticsSourceDurationMapBuilder(ServiceIdentity service, string traceId, string name, InMemoryRequestDiagnosticsSource diagnosticsSource, string? parentId = null) : base(service, traceId, Guid.NewGuid().ToString(), name, diagnosticsSource)
    {
        this.diagnosticsSource = diagnosticsSource;
        this.parentId = parentId;
    }

    public IDurationSegmentBuilder StartNewSegment(ServiceIdentity service, string segmentName, DateTimeOffset startTime) => StartNested(service, segmentName, startTime);

    public IDurationSegmentBuilder StartNewSegment(string segmentName) => StartNested(segmentName);

    public IDurationSegmentBuilder StartNewSegment(string segmentName, DateTimeOffset startTime) => StartNested(segmentName, startTime);

    // Root element is not really existing so it does not required to notify anyone.
    void IDurationMapBuilder.End() =>
        EndTime = DateTimeOffset.UtcNow;

    public override IDurationSegmentBuilder StartNested(ServiceIdentity service, string segmentName, DateTimeOffset start)
    {
        string nestedId = IdGeneratorHelper.GenerateNewSpanId();
        diagnosticsSource.StartActivity(new ActivityStart(service, TraceId, parentId, nestedId, segmentName, start));
        var nestedSegment = new InMemoryDiagnosticsSourceDurationSegmentBuilder(this, TraceId, nestedId, segmentName, service, diagnosticsSource);
        nestedSegment.Start(start);
        return nestedSegment;
    }
}
