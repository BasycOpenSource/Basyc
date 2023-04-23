﻿using System.Diagnostics;
using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Producing.Abstractions;

public struct ActivityStartDisposer : IDisposable
{
    private readonly IDiagnosticsExporter diagnosticsProducer;

    private bool isEnded = false;

    public ActivityStartDisposer(IDiagnosticsExporter diagnosticsProducer, ActivityStart activityStart)
    {
        this.diagnosticsProducer = diagnosticsProducer;
        ActivityStart = activityStart;
    }

    public ActivityStart ActivityStart { get; init; }

    public void Dispose()
    {
        if (isEnded)
            return;

        diagnosticsProducer.EndActivity(ActivityStart, DateTimeOffset.UtcNow);
        isEnded = true;
    }

    public void Stop(DateTimeOffset endTime = default, ActivityStatusCode activityStatusCode = ActivityStatusCode.Ok)
    {
        if (isEnded)
            throw new InvalidOperationException("Activity is already ended");

        diagnosticsProducer.EndActivity(ActivityStart, endTime, activityStatusCode);
        isEnded = true;
    }

    public ActivityStartDisposer StartNested(string name, DateTimeOffset startTime = default)
    {
        var nestedActivityDisposer = diagnosticsProducer.StartActivity(this, name, startTime);
        return nestedActivityDisposer;
    }
}
