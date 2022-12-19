using Basyc.MessageBus.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.DomainDrivenDesign.Domain
{
    public interface IQuery<TResponse> : IMessage<TResponse> where TResponse : class
    {
    }
}