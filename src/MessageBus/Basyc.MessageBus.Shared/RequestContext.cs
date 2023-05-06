namespace Basyc.MessageBus.Shared;

/// <summary>
/// RequestContext summary.
/// </summary>
/// <param name="TraceId">When not specifying Trace id, trace id will be generated.</param>
public record struct RequestContext(string ParentSpanId, string TraceId);
