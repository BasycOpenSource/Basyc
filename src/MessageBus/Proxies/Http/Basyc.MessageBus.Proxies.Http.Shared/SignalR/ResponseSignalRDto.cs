namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;

#pragma warning disable CA1819 // Properties should not return arrays
public record ResponseSignalRDto(string TraceId, bool HasResponse, byte[]? ResponseData = null, string? ResponseType = null);
