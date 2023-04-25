namespace Basyc.MessageBus.Shared;

public interface IMessage
{
}

public interface IMessage<TResponse>
    where TResponse : class
{
}
