﻿using Basyc.Diagnostics.Shared.Logging;

namespace Basyc.Diagnostics.Producing.Abstractions;

#pragma warning disable CA1003 // Use generic event handler instances

public class InMemoryDiagnosticsExporter : IDiagnosticsExporter
{
    public event EventHandler<LogEntry>? LogProduced;

    public event EventHandler<ActivityStart>? StartProduced;

    public event EventHandler<ActivityEnd>? EndProduced;

    public Task ProduceLog(LogEntry logEntry)
    {
        LogProduced?.Invoke(this, logEntry);
        return Task.CompletedTask;
    }

    public Task StartActivity(ActivityStart activityStart)
    {
        StartProduced?.Invoke(this, activityStart);
        return Task.CompletedTask;
    }

    public Task EndActivity(ActivityEnd activityEnd)
    {
        EndProduced?.Invoke(this, activityEnd);
        return Task.CompletedTask;
    }

    public Task<bool> StartAsync() => Task.FromResult(true);
}
