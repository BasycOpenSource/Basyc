namespace Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall;

public class MethodsServerCanCallPrivateMethods
{
	public const string ReceiveTextMethodName = nameof(ReceiveText);

	public const string ReceiveVoidMethodName = nameof(ReceiveVoid);

	public string? LastReceivedText { get; private set; }
	public DateTime LastReceivedVoidUtc { get; private set; }

	private void ReceiveText(string text)
	{
		LastReceivedText = text;
	}

	private void ReceiveVoid()
	{
		LastReceivedVoidUtc = DateTime.UtcNow;
	}
}
