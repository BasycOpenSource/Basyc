namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;

public record ResponseSignalRDto(string TraceId, bool HasResponse, byte[]? ResponseData = null, string? ResponseType = null);
