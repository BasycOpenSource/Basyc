using Dapr.Client;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basyc.MicroService.Dapr.MessageBus.Handlers;

public abstract class DaprRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	private readonly DaprClient daprClient;
	private readonly string appName;

	public DaprRequestHandler(DaprClient daprClient, string appName)
	{
		this.daprClient = daprClient;
		this.appName = appName;
	}
	public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
	{
		return daprClient.InvokeMethodAsync<TRequest, TResponse>(appName, nameof(TResponse), request, cancellationToken);
	}
}