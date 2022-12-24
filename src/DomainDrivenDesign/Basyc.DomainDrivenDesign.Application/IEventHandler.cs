using Basyc.MessageBus.Client.RequestResponse;
using Basyc.MessageBus.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.DomainDrivenDesign.Application;

public interface IEventHandler<TEvent> : IMessageHandler<TEvent>
	where TEvent : class, IEvent
{
}
