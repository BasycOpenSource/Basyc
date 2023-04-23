using Basyc.Diagnostics.Shared.Durations;
using System;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class ResultLoggingContextLoggerScopeEnder : IDisposable
{
    private readonly InMemoryDurationSegmentBuilder durationSegmentBuilder;

    public ResultLoggingContextLoggerScopeEnder(InMemoryDurationSegmentBuilder durationSegmentBuilder)
    {
        this.durationSegmentBuilder = durationSegmentBuilder;
    }
    public void Dispose() => durationSegmentBuilder.End();
}
