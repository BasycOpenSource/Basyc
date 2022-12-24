using Basyc.MessageBus.Shared;

namespace Basyc.MessageBus.HttpProxy.Shared.SignalR;

public record RequestSignalRDTO(string MessageType,
							 bool HasResponse,
							 byte[]? MessageBytes = null,
							 string? ResponseType = null,
							 RequestContext RequestContext = default);
