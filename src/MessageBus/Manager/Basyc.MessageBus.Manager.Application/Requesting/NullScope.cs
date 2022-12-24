using System;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class NullScope : IDisposable
{
	public static NullScope Instance { get; } = new NullScope();
	public void Dispose()
	{

	}
}
