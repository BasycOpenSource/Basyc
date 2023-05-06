using Basyc.MessageBus.Shared;

namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;
#pragma warning disable CA1819 // Properties should not return arrays

public record RequestSignalRDto(string MessageType,
    bool HasResponse,
    byte[]? MessageBytes = null,
    string? ResponseType = null,
    RequestContext RequestContext = default);
