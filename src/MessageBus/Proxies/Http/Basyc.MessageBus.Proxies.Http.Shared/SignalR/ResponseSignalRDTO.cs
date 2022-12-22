namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;

public record ResponseSignalRDTO(string TraceId, bool HasResponse, byte[]? ResponseData = null, string? ResponseType = null);