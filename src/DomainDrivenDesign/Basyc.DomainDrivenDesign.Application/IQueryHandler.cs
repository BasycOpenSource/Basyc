using Basyc.DomainDrivenDesign.Domain;
using Basyc.MessageBus.Client.RequestResponse;

namespace Basyc.DomainDrivenDesign.Application;

public interface IQueryHandler<TQuery, TReponse> : IMessageHandler<TQuery, TReponse>
  where TQuery : class, IQuery<TReponse>
  where TReponse : class
{
}
