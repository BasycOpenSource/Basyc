namespace Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall
{
	public class MethodsServerCanCall_Numbers : IMethodsServerCanCall_Empty, IMethodsServerCanCall_Numbers
	{
		public int LastReceivedNumber { get; private set; }

		public void ReceiveNumber(int number)
		{
			LastReceivedNumber = number;
		}

		public async Task ReceiveNumberAsync(int number)
		{
			LastReceivedNumber = number;
			await Task.Delay(150);
		}

		public async Task ReceiveNumbers(int number, int number2)
		{
			LastReceivedNumber = number2;
			await Task.Delay(150);
		}

		public async Task ReceiveNumbers(int number, int number2, int number3)
		{
			LastReceivedNumber = number3;
			await Task.Delay(150);
		}

	}
}
