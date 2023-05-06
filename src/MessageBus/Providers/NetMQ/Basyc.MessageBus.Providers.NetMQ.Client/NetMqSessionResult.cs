namespace Basyc.MessageBus.Client.NetMQ;

#pragma warning disable CA1819 // Properties should not return arrays
public record NetMqSessionResult(byte[] Bytes, string ResponseType);
