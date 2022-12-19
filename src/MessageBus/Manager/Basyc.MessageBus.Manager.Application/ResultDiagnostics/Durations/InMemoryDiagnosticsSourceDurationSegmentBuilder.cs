using Basyc.Diagnostics.Shared.Durations;
using Basyc.Diagnostics.Shared.Helpers;
using Basyc.Diagnostics.Shared.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Application.ResultDiagnostics.Durations
{
    internal class InMemoryDiagnosticsSourceDurationSegmentBuilder : DurationSegmentBuilderBase
    {
        private readonly InMemoryRequestDiagnosticsSource diagnosticsSource;
        private readonly InMemoryDiagnosticsSourceDurationSegmentBuilder? parent;

        public InMemoryDiagnosticsSourceDurationSegmentBuilder(ServiceIdentity service, string traceId, string id, string name, InMemoryRequestDiagnosticsSource diagnosticsSource)
            : base(service, traceId, id, name)
        {
            this.diagnosticsSource = diagnosticsSource;
        }

        public InMemoryDiagnosticsSourceDurationSegmentBuilder(InMemoryDiagnosticsSourceDurationSegmentBuilder parent, string traceId, string id, string name, ServiceIdentity service, InMemoryRequestDiagnosticsSource diagnosticsSource)
            : base(service, traceId, id, name)
        {
            this.diagnosticsSource = diagnosticsSource;
            this.parent = parent;
            HasParent = true;
        }

        public override void End(DateTimeOffset finalEndTime)
        {
            EndTime = finalEndTime;
            diagnosticsSource.EndActivity(new ActivityEnd(Service, TraceId, parent?.Id, Id, Name, StartTime, EndTime, System.Diagnostics.ActivityStatusCode.Ok));
        }

        public override IDurationSegmentBuilder EndAndStartFollowing(string segmentName)
        {
            if (HasParent is false)
                throw new InvalidOperationException("Segment must have a parent in order to start following segment");

            var endTime = End();
            return parent!.StartNested(segmentName, endTime);
        }
        public override ValueTask Log(string message, LogLevel logLevel)
        {
            diagnosticsSource.PushLog(new LogEntry(this.Service, this.TraceId, DateTimeOffset.UtcNow, logLevel, message, this.Id));
            return ValueTask.CompletedTask;
        }

        public override IDurationSegmentBuilder StartNested(ServiceIdentity service, string segmentName, DateTimeOffset start)
        {
            var nestedId = IdGeneratorHelper.GenerateNewSpanId();
            diagnosticsSource.StartActivity(new ActivityStart(service, TraceId, Id, nestedId, segmentName, start));
            return new InMemoryDiagnosticsSourceDurationSegmentBuilder(this, TraceId, nestedId, segmentName, service, diagnosticsSource);
        }
    }
}