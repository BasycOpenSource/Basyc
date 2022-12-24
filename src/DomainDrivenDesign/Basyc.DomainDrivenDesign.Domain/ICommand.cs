using Basyc.MessageBus.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.DomainDrivenDesign.Domain;

public interface ICommand : IMessage
{
}

public interface ICommand<TResponse> : IMessage<TResponse> where TResponse : class
{
}

//public interface ICommandWithContext<TContext> : ICommand, IRequestWithContext<TContext>
//{
//}

//public interface ICommandWithContext<TResponse, TContext> : ICommand<TResponse>, IRequestWithContext<TResponse, TContext>
//    where TResponse : class
//{
//}
