namespace Basyc.MessageBus.HttpProxy.Client.Http;

public record ProxyResponse(object? Response, bool HasResponse, bool HasError, string? TraceId);