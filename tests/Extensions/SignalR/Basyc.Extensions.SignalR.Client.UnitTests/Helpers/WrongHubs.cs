namespace Basyc.Extensions.SignalR.Client.Tests.Helpers;

public static class WrongHubs
{
	public static Type[] TypesThatShouldFailCreating = new Type[]
	{
		typeof(IWrongHubClient_Has_ReturnValues),
		typeof(IWrongHubClient_Has_TaskReturnValues),
		typeof(IWrongHubClient_HasInherited_ReturnValue),
		typeof(IWrongHubClient_Has_AllWrongs),
		typeof(WrongHubClient_Is_Class),
	};
}

public interface IWrongHubClient_Has_ReturnValues : ICorrectMethodsClientCanCall_Voids
{
	int WrongSendNothingReceiveNumber();
	string WrongSendNothingReceiveText();
	int WrongSendNothing();
	int WrongSendNothing2();
	string WrongSendInt(int number);
	object WrongSendIntString(int number, string name);
}

public interface IWrongHubClient_Has_TaskReturnValues : ICorrectMethodsClientCanCall_Voids
{
	Task<int> WrongSendNothingAsyncInt();
	Task<int> WrongSendIntAsyncInt(int number);
	Task<int> WrongSendIntCancelAsyncInt(int number, CancellationToken cancellationToken);
	Task<int> WrongSendIntStringAsyncInt(int number, string text);
}

public interface IWrongHubClient_HasInherited_ReturnValue : IWrongHubClient_Has_ReturnValues
{
}

public interface IWrongHubClient_Has_AllWrongs :
	IWrongHubClient_Has_ReturnValues,
	IWrongHubClient_Has_TaskReturnValues,
	ICorrectMethodsClientCanCall_AllCorrect
{
}

public class WrongHubClient_Is_Class
{

}

public class WrongHubClient_Is_Class_Numbers_Exceptions
{
	public void ThrowNumberVoid(int number)
	{
		throw MethodExceptionHelperException.CreateForCurrentMethod();
	}

	public async Task ThrowNumberAsync(int number)
	{
		await Task.Delay(150);
		throw MethodExceptionHelperException.CreateForCurrentMethod(new object?[] { number });
	}

	public async Task ThrowNumbersAsync(int number, int number2)
	{
		await Task.Delay(150);
		throw MethodExceptionHelperException.CreateForCurrentMethod(new object?[] { number, number2 });

	}

	public async Task ThrowNumbers3Async(int number, int number2, int number3)
	{
		await Task.Delay(150);
		throw MethodExceptionHelperException.CreateForCurrentMethod(new object?[] { number, number2, number3 });
	}
}
