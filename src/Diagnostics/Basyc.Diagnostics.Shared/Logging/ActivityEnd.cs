using Basyc.Diagnostics.Shared.Durations;
using System.Diagnostics;

namespace Basyc.Diagnostics.Shared.Logging;

public readonly struct ActivityEnd
{
    public ActivityEnd(ServiceIdentity service,
        string traceId,
        string? parentId,
        string id,
        string name,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        ActivityStatusCode status)
    {
        if (parentId is not null && parentId == id)
            throw new ArgumentException($"{nameof(parentId)} cant be the same as {nameof(id)}");

        this.Service = service;
        this.TraceId = traceId;
        this.ParentId = parentId;
        this.Id = id;
        this.Name = name;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.Status = status;
    }

    public ServiceIdentity Service { get; init; }
    public string TraceId { get; init; }
    public string? ParentId { get; init; }
    public string Id { get; init; }
    public string Name { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }
    public ActivityStatusCode Status { get; init; }

    public bool HasParent => ParentId != null;

}
