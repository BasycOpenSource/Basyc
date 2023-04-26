using Dapr.Client;
using MediatR;

namespace Basyc.MicroService.Dapr.MessageBus.Handlers;

public abstract class DaprRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly DaprClient daprClient;
    private readonly string appName;

    protected DaprRequestHandler(DaprClient daprClient, string appName)
    {
        this.daprClient = daprClient;
        this.appName = appName;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken) => daprClient.InvokeMethodAsync<TRequest, TResponse>(appName, nameof(TResponse), request, cancellationToken);
}
