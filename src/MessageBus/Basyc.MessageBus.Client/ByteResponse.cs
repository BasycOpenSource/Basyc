namespace Basyc.MessageBus.Client;

/// <summary>
/// Contains bytes and its metadata (message type - that can be used for deserialization).
/// </summary>
public record ByteResponse(byte[] ResponseBytes, string ResponseType);
