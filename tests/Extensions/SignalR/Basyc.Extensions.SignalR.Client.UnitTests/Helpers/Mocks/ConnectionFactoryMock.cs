using Microsoft.AspNetCore.Connections;
using System.IO.Pipelines;
using System.Net;

namespace Basyc.Extensions.SignalR.Client.Tests.Mocks;

public class ConnectionFactoryMock : IConnectionFactory
{
	private readonly Pipe pipe;

	public ConnectionFactoryMock(Pipe pipe)
	{
		this.pipe = pipe;
	}
	public ValueTask<ConnectionContext> ConnectAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
	{
		return new ValueTask<ConnectionContext>(new ConnectionContextMock(pipe));
	}
}