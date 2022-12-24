namespace Basyc.MessageBus.Shared;

/// <summary>
/// 
/// </summary>
/// <param name="ParentSpanId"></param>
/// <param name="TraceId">When not specifiyng Trace id, trace id will be generated</param>
public record struct RequestContext(string ParentSpanId, string TraceId);
