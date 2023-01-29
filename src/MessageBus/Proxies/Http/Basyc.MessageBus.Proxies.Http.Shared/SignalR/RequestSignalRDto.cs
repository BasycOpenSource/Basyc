using Basyc.MessageBus.Shared;

namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;

public record RequestSignalRDto(string MessageType,
	bool HasResponse,
	byte[]? MessageBytes = null,
	string? ResponseType = null,
	RequestContext RequestContext = default);
