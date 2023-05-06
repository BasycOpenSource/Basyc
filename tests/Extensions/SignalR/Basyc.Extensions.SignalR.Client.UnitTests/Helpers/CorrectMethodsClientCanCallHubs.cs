namespace Basyc.Extensions.SignalR.Client.Tests.Helpers;
#pragma warning disable SA1649
#pragma warning disable SA1201

public interface ICorrectMethodsClientCanCallNoMethods
{
}

public static class CorrectMethodsClientCanCallHubs
{
    public static readonly Type[] CorrectMethodsClientCanCallTypes =
    {
        typeof(ICorrectMethodsClientCanCallNoMethods),
        typeof(ICorrectMethodsClientCanCallVoids),
        typeof(ICorrectMethodsClientCanCallTasks),
        typeof(ICorrectMethodsClientCanCallInheritedVoids),
        typeof(ICorrectMethodsClientCanCallAllCorrect)
    };
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
