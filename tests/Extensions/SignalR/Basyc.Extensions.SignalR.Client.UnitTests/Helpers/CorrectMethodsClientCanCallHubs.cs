namespace Basyc.Extensions.SignalR.Client.Tests.Helpers;

public static class CorrectMethodsClientCanCallHubs
{
	public static Type[] CorrectMethodsClientCanCallTypes = new Type[]
	{
		typeof(ICorrectMethodsClientCanCall_NoMethods),
		typeof(ICorrectMethodsClientCanCall_Voids),
		typeof(ICorrectMethodsClientCanCall_Tasks),
		typeof(ICorrectMethodsClientCanCall_Inherited_Voids),
		typeof(ICorrectMethodsClientCanCall_AllCorrect),
	};
}

public interface ICorrectMethodsClientCanCall_NoMethods
{

}

public interface ICorrectMethodsClientCanCall_Voids
{
	void SendNothing();
	void SendNumber(int number);
	void SendIntString(int number, string name);
}

public interface ICorrectMethodsClientCanCall_Tasks
{
	Task SendNothingAsync();
	Task SendIntAsync(int number);
	Task SendIntCancelAsync(int number, CancellationToken cancellationToken);
	Task SendIntStringCancelAsync(int number, string name, CancellationToken cancellationToken);
}

public interface ICorrectMethodsClientCanCall_Inherited_Voids : ICorrectMethodsClientCanCall_Voids
{

}

public interface ICorrectMethodsClientCanCall_AllCorrect :
	ICorrectMethodsClientCanCall_NoMethods,
	ICorrectMethodsClientCanCall_Voids,
	ICorrectMethodsClientCanCall_Tasks
{
}
