using Basyc.MessageBus.Shared;
using MassTransit;
using OneOf;
using System.Reflection;
using Throw;

namespace Basyc.MessageBus.Client.MasstTransit;
#pragma warning disable CA1033 // Interface methods should be callable by child types

public class MassTransitMessageBusClient : ITypedMessageBusClient
{
    private readonly IBusControl massTransitBus;

    public MassTransitMessageBusClient(IBusControl massTransitBus)
    {
        this.massTransitBus = massTransitBus;
    }

    BusTask ITypedMessageBusClient.PublishAsync<TEvent>(RequestContext requestContext, CancellationToken cancellationToken)
    {
        var publishTask = massTransitBus.Publish<TEvent>(new(), cancellationToken);
        return BusTask.FromTask("-1", publishTask);
    }

    BusTask ITypedMessageBusClient.PublishAsync<TEvent>(TEvent data, RequestContext requestContext, CancellationToken cancellationToken)
    {
        var publishTask = massTransitBus.Publish(data, cancellationToken);
        return BusTask.FromTask("-1", publishTask);
    }

    BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(RequestContext requestContext, CancellationToken cancellationToken)
    {
        var requestTask = massTransitBus.Request<TRequest, TResponse>(cancellationToken);
        return BusTask<TResponse>.FromTask<Response<TResponse>>("-1", requestTask, x => (OneOf<TResponse, ErrorMessage>)x.Message);
    }

    BusTask<TResponse> ITypedMessageBusClient.RequestAsync<TRequest, TResponse>(
        TRequest request,
        RequestContext requestContext,
        CancellationToken cancellationToken)
    {
        var requestTask = massTransitBus.Request<TRequest, TResponse>(request, cancellationToken);
        return BusTask<TResponse>.FromTask<Response<TResponse>>("-1", requestTask, x => (OneOf<TResponse, ErrorMessage>)x.Message);
    }

    BusTask<object> ITypedMessageBusClient.RequestAsync(Type requestType, Type responseType, RequestContext requestContext, CancellationToken cancellationToken)
    {
        var bus = (ITypedMessageBusClient)this;
        var requestData = Activator.CreateInstance(requestType);
        requestData.ThrowIfNull();
        return bus.RequestAsync(requestType, requestData, responseType, requestContext, cancellationToken);
    }

    public BusTask<object> RequestAsync(Type requestType,
        object requestData,
        Type responseType,
        RequestContext requestContext = default,
        CancellationToken cancellationToken = default)
    {
        var conType = typeof(SendContext<>).MakeGenericType(requestType);
        var actionType = typeof(Action<>).MakeGenericType(conType);
        var methodParameterTypes = new[] { typeof(IBus), requestType, typeof(CancellationToken), typeof(RequestTimeout), actionType };
        //MassTransit does not have Request variant that accepts type as parameter (not type parameter)
        //MethodInfo requestMethodInfo = typeof(MassTransit.RequestExtensions).GetMethod(nameof(MassTransit.RequestExtensions.Request), BindingFlags.Public | BindingFlags.Static, null, methodParameterTypes, null);
        var requestMethodInfo = typeof(RequestExtensions).GetMethods().Where(x => x.Name == nameof(RequestExtensions.Request)).Skip(2).First();
        var parameters = requestMethodInfo.GetParameters();
        var genericMethod = requestMethodInfo.MakeGenericMethod(requestType, responseType);

        //var busResponse = (Response<object>)await InvokeAsync(genericMethod, null, new object[] { massTransitBus, request, cancellationToken, default(RequestTimeout), null });
        //return busResponse.Message;

        var busResponse = InvokeAsync(genericMethod, null, massTransitBus, requestData, cancellationToken, default(RequestTimeout), null!);
        var busTask = BusTask<object>.FromTask("-1", busResponse);
        return busTask;
    }

    public BusTask SendAsync(
        Type commandType,
        object commandData,
        RequestContext requestContext = default,
        CancellationToken cancellationToken = default) =>
        //await _massTransitBus.Publish(request, requestType, cancellationToken); //Wont get response
        //await _massTransitBus.Send(request, requestType, cancellationToken); //Does not work

        //Command can return response, but should not query data, returning command completion status is allowed
        RequestAsync(commandType, commandData, typeof(VoidCommandResult), requestContext, cancellationToken).ToBusTask();

    public BusTask SendAsync(Type commandType, RequestContext requestContext = default, CancellationToken cancellationToken = default)
    {
        var request = Activator.CreateInstance(commandType);
        request.ThrowIfNull();
        //await _massTransitBus.Publish(request, requestType, cancellationToken);
        return SendAsync(commandType, request, requestContext, cancellationToken);
    }

    BusTask ITypedMessageBusClient.SendAsync<TRequest>(RequestContext requestContext, CancellationToken cancellationToken) =>
        //await _massTransitBus.Publish<TRequest>(cancellationToken);
        SendAsync(typeof(TRequest), requestContext, cancellationToken);

    BusTask ITypedMessageBusClient.SendAsync<TRequest>(TRequest request, RequestContext requestContext, CancellationToken cancellationToken) =>
        //await _massTransitBus.Publish(request, cancellationToken);
        SendAsync(typeof(TRequest), request, requestContext, cancellationToken);

    public void Dispose()
    {
        try
        {
            massTransitBus.Start(new TimeSpan(0, 0, 30));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("MassTransit timeout", ex);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    private static async Task<object> InvokeAsync(MethodInfo masstransitRequestMethod, object? obj, params object[]? parameters)
    {
        var task = (Task)masstransitRequestMethod.Invoke(obj, parameters)!;
        await task.ConfigureAwait(false);
#pragma warning disable CA1849 // Call async methods when in an async method
        var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result));
#pragma warning restore CA1849 // Call async methods when in an async method
        resultProperty.ThrowIfNull();
        var value = resultProperty.GetValue(task);
        value.ThrowIfNull();
        return value;
    }
}
