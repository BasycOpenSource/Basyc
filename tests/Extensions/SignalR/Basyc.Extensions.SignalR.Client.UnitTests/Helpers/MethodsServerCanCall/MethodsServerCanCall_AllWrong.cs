namespace Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall;

public class MethodsServerCanCall_AllWrong
{

	public MethodsServerCanCall_AllWrong()
	{

	}

	private MethodsServerCanCall_AllWrong(int number)
	{

	}

	public int PublicIntAutoProperty { get; set; }
	private readonly int privateIntField;

	public string? PublicFullStringAutoProperty { get => privateStringString; set => privateStringString = value; }
	private string? privateStringString;

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
