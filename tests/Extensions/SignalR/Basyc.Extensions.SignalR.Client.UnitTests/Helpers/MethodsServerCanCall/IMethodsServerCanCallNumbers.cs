namespace Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall;

public interface IMethodsServerCanCallNumbers
{
    void ReceiveNumber(int number);

    Task ReceiveNumbers(int number, int number2);

    Task ReceiveNumbers(int number, int number2, int number3);
}
