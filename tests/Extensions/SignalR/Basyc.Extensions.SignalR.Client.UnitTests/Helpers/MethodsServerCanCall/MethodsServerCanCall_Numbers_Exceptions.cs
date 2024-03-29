﻿using Basyc.Extensions.SignalR.Client.Tests.Helpers;

namespace Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall;

public class MethodsServerCanCall_Numbers_Exceptions : IMethodsServerCanCall_Empty, IMethodsServerCanCall_Numbers
{

	public void ReceiveNumber(int number)
	{
		throw MethodExceptionHelperException.CreateForCurrentMethod();
	}

	public async Task ReceiveNumberAsync(int number)
	{
		await Task.Delay(150);
		throw MethodExceptionHelperException.CreateForCurrentMethod(new object?[] { number });
	}

	public async Task ReceiveNumbers(int number, int number2)
	{
		await Task.Delay(150);
		throw MethodExceptionHelperException.CreateForCurrentMethod(new object?[] { number, number2 });

	}

	public async Task ReceiveNumbers(int number, int number2, int number3)
	{
		await Task.Delay(150);
		throw MethodExceptionHelperException.CreateForCurrentMethod(new object?[] { number, number2, number3 });
	}
}
