namespace Basyc.Extensions.SignalR.Client.Tests.Helpers;

public static class CorrectMethodsClientCanCallHubs
{
	public static Type[] CorrectMethodsClientCanCallTypes =
	{
		typeof(ICorrectMethodsClientCanCallNoMethods),
		typeof(ICorrectMethodsClientCanCallVoids),
		typeof(ICorrectMethodsClientCanCallTasks),
		typeof(ICorrectMethodsClientCanCallInheritedVoids),
		typeof(ICorrectMethodsClientCanCallAllCorrect)
	};
}

public interface ICorrectMethodsClientCanCallNoMethods
{
}

public interface ICorrectMethodsClientCanCallVoids
{
	void SendNothing();
	void SendNumber(int number);
	void SendIntString(int number, string name);
}

public interface ICorrectMethodsClientCanCallTasks
{
	Task SendNothingAsync();
	Task SendIntAsync(int number);
	Task SendIntCancelAsync(int number, CancellationToken cancellationToken);
	Task SendIntStringCancelAsync(int number, string name, CancellationToken cancellationToken);
}

public interface ICorrectMethodsClientCanCallInheritedVoids : ICorrectMethodsClientCanCallVoids
{
}

public interface ICorrectMethodsClientCanCallAllCorrect :
	ICorrectMethodsClientCanCallNoMethods,
	ICorrectMethodsClientCanCallVoids,
	ICorrectMethodsClientCanCallTasks
{
}
