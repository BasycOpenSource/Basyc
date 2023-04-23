using Basyc.MessageBus.Shared;

namespace Basyc.DomainDrivenDesign.Domain;

public interface ICommand : IMessage
{
}

public interface ICommand<TResponse> : IMessage<TResponse> where TResponse : class
{
}
