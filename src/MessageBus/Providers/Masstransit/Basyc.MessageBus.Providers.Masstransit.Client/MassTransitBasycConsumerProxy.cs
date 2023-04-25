using Basyc.MessageBus.Client.RequestResponse;
using Basyc.MessageBus.Shared;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Client.MasstTransit;

public class MassTransitBasycConsumerProxy<TRequest> : IConsumer<TRequest>
    where TRequest : class, IMessage
{
    private readonly IMessageHandler<TRequest> requestHandler;

    public MassTransitBasycConsumerProxy(IMessageHandler<TRequest> requestHandler)
    {
        this.requestHandler = requestHandler;
    }

    public async Task Consume(ConsumeContext<TRequest> context)
    {
        await requestHandler.Handle(context.Message, context.CancellationToken);
        await context.RespondAsync(new VoidCommandResult());
    }
}

#pragma warning disable SA1402
public class MassTransitBasycConsumerProxy<TRequest, TResponse> : IConsumer<TRequest>
#pragma warning restore SA1402
    where TRequest : class, IMessage<TResponse>
    where TResponse : class
{
    private readonly IMessageHandler<TRequest, TResponse> requestHandler;

    public MassTransitBasycConsumerProxy(IMessageHandler<TRequest, TResponse> requestHandler)
    {
        this.requestHandler = requestHandler;
    }

    public async Task Consume(ConsumeContext<TRequest> context)
    {
        var response = await requestHandler.Handle(context.Message, context.CancellationToken);
        await context.RespondAsync(response);
    }
}
