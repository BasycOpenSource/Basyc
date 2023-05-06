using Basyc.MessageBus.Client.RequestResponse;
using Basyc.MessageBus.Shared;

namespace Basyc.DomainDrivenDesign.Application;

#pragma warning disable CA1711
public interface IEventHandler<TEvent> : IMessageHandler<TEvent>
    where TEvent : class, IEvent
{
}
