using Basyc.MessageBus.Client.RequestResponse;
using Basyc.MessageBus.Shared;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Client.Building;
public class MessageDelegateHandler<TMessage> : IMessageHandler<TMessage>
    where TMessage : IMessage
{
    private readonly ILogger logger;
    private readonly Action<TMessage, ILogger> handlerDelegate;

    public MessageDelegateHandler(ILogger logger, Action<TMessage, ILogger> handlerDelegate)
    {
        this.logger = logger;
        this.handlerDelegate = handlerDelegate;
    }

    public Task Handle(TMessage message, CancellationToken cancellationToken = default)
    {
        handlerDelegate.Invoke(message, logger);
        return Task.CompletedTask;
    }
}
