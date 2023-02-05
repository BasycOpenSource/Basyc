namespace Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall;

public class MethodsServerCanCallAllWrong
{
	private readonly int privateIntField;

	public MethodsServerCanCallAllWrong()
	{
	}

	private MethodsServerCanCallAllWrong(int number)
	{
	}

	public int PublicIntAutoProperty { get; set; }

	public string? PublicFullStringAutoProperty { get; set; }

	private void PrivateVoidMethod()
	{
	}

	private void PrivateVoidMethodInt(int number)
	{
	}

	private Task PrivateTaskMethod()
	{
		return Task.CompletedTask;
	}

	private Task PrivateTaskMethodInt(int number)
	{
		return Task.CompletedTask;
	}
}
