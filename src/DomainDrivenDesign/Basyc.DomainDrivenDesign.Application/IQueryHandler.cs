using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basyc.DomainDrivenDesign.Domain;
using Basyc.MessageBus.Client.RequestResponse;

namespace Basyc.DomainDrivenDesign.Application
{
    public interface IQueryHandler<TQuery, TReponse> : IMessageHandler<TQuery, TReponse>
      where TQuery : class, IQuery<TReponse>
      where TReponse : class
    {
    }
}