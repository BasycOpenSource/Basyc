namespace Basyc.Extensions.SignalR.Client.Tests.Helpers;
#pragma warning disable SA1201
#pragma warning disable SA1649
#pragma warning disable SA1402
#pragma warning disable IDE0060 // Remove unused parameter

public interface IWrongHubClientHasReturnValues : ICorrectMethodsClientCanCallVoids
{
    int WrongSendNothingReceiveNumber();

    string WrongSendNothingReceiveText();

    int WrongSendNothing();

    int WrongSendNothing2();

    string WrongSendInt(int number);

    object WrongSendIntString(int number, string name);
}

public static class WrongHubs
{
    public static readonly Type[] TypesThatShouldFailCreating =
    {
        typeof(IWrongHubClientHasReturnValues),
        typeof(IWrongHubClientHasTaskReturnValues),
        typeof(IWrongHubClientHasInheritedReturnValue),
        typeof(IWrongHubClientHasAllWrongs),
        typeof(WrongHubClientIsClass)
    };
}

public interface IWrongHubClientHasTaskReturnValues : ICorrectMethodsClientCanCallVoids
{
    Task<int> WrongSendNothingAsyncInt();

    Task<int> WrongSendIntAsyncInt(int number);

    Task<int> WrongSendIntCancelAsyncInt(int number, CancellationToken cancellationToken);

    Task<int> WrongSendIntStringAsyncInt(int number, string text);
}

public interface IWrongHubClientHasInheritedReturnValue : IWrongHubClientHasReturnValues
{
}

public interface IWrongHubClientHasAllWrongs :
    IWrongHubClientHasReturnValues,
    IWrongHubClientHasTaskReturnValues,
    ICorrectMethodsClientCanCallAllCorrect
{
}

public class WrongHubClientIsClass
{
}

public class WrongHubClientIsClassNumbersExceptions
{
    public static void ThrowNumberVoid(int number) => throw MethodExceptionHelperException.CreateForCurrentMethod();

    public static async Task ThrowNumberAsync(int number)
    {
        await Task.Delay(150);
        throw MethodExceptionHelperException.CreateForCurrentMethod(new object?[] { number });
    }

    public static async Task ThrowNumbersAsync(int number, int number2)
    {
        await Task.Delay(150);
        throw MethodExceptionHelperException.CreateForCurrentMethod(new object?[] { number, number2 });
    }

    public static async Task ThrowNumbers3Async(int number, int number2, int number3)
    {
        await Task.Delay(150);
        throw MethodExceptionHelperException.CreateForCurrentMethod(new object?[] { number, number2, number3 });
    }
}
