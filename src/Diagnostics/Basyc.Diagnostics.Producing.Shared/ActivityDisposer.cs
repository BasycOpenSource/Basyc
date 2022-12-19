using Basyc.Diagnostics.Shared.Logging;
using System.Diagnostics;

namespace Basyc.Diagnostics.Producing.Shared
{
    public struct ActivityDisposer : IDisposable
    {
        private readonly IDiagnosticsExporter diagnosticsProducer;
        private bool isEnded = false;
        public ActivityStart ActivityStart { get; init; }

        public ActivityDisposer(IDiagnosticsExporter diagnosticsProducer, ActivityStart activityStart)
        {
            this.diagnosticsProducer = diagnosticsProducer;
            this.ActivityStart = activityStart;
        }


        public void Dispose()
        {
            if (isEnded)
                return;

            diagnosticsProducer.EndActivity(ActivityStart, DateTimeOffset.UtcNow);
            isEnded = true;
        }

        public void End(DateTimeOffset endTime = default, ActivityStatusCode activityStatusCode = ActivityStatusCode.Ok)
        {
            if (isEnded)
                throw new InvalidOperationException("Activity is already ended");

            diagnosticsProducer.EndActivity(ActivityStart, endTime, activityStatusCode);
            isEnded = true;
        }

        public ActivityDisposer StartNested(string name, DateTimeOffset startTime = default)
        {
            var nestedActivityDisposer = diagnosticsProducer.StartActivity(this, name, startTime);
            return nestedActivityDisposer;

        }
    }
}