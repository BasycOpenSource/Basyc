using Basyc.DomainDrivenDesign.Domain;
using Basyc.MessageBus.Client.RequestResponse;

namespace Basyc.DomainDrivenDesign.Application;

public interface ICommandHandler<TCommand> : IMessageHandler<TCommand>
    where TCommand : class, ICommand
{
}

public interface ICommandHandler<TCommand, TReponse> : IMessageHandler<TCommand, TReponse>
    where TCommand : class, ICommand<TReponse>
    where TReponse : class
{
}
