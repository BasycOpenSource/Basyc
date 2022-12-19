namespace Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall
{
    public interface IMethodsServerCanCall_Numbers
    {
        void ReceiveNumber(int number);
        Task ReceiveNumbers(int number, int number2);
        Task ReceiveNumbers(int number, int number2, int number3);
    }
}