namespace Basyc.MessageBus.Client
{
	/// <summary>
	/// Contains bytes and its metadata (message type - that can be used for deserailization)
	/// </summary>
	/// <param name="ResponseBytes"></param>
	/// <param name="MessageType"></param>
	public record ByteResponse(byte[] ResponseBytes, string ResposneType);
}
