using Basyc.MessageBus.Shared;

namespace Basyc.DomainDrivenDesign.Domain;

public interface IQuery<TResponse> : IMessage<TResponse> where TResponse : class
{
}
