namespace Basyc.MessageBus.Client;

/// <summary>
/// Contains bytes and its metadata (message type - that can be used for deserialization).
/// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
public record ByteResponse(byte[] ResponseBytes, string ResponseType);
