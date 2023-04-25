﻿using Basyc.MessageBus.Shared;

namespace Basyc.MessageBus.Client.RequestResponse;

public interface IMessageHandler<TMessage> where TMessage : IMessage
{
    Task Handle(TMessage message, CancellationToken cancellationToken = default);
}

public interface IMessageHandler<TMessage, TResponse>
    where TMessage : IMessage<TResponse>
    where TResponse : class
{
    Task<TResponse> Handle(TMessage message, CancellationToken cancellationToken = default);
}
