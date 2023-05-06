namespace Basyc.Diagnostics.Shared.Logging;

#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct ActivityStart
{
    public ActivityStart(
        ServiceIdentity service,
        string traceId,
        string? parentId,
        string id,
        string name,
        DateTimeOffset startTime)
    {
        if (parentId is not null && parentId == id)
            throw new ArgumentException($"{nameof(parentId)} cant be the same as {nameof(id)}");

        Service = service;
        TraceId = traceId;
        ParentId = parentId;
        Id = id;
        Name = name;
        StartTime = startTime;
    }

    public ServiceIdentity Service { get; init; }

    public string TraceId { get; init; }

    public string? ParentId { get; init; }

    public string Id { get; init; }

    public string Name { get; init; }

    public DateTimeOffset StartTime { get; init; }

    public bool HasParent => ParentId != null;
}
