using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Shared;

public interface IMessage
{
}

public interface IMessage<TResponse>
    where TResponse : class
{
}
