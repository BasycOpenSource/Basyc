using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.HandledByStages;

public class FluentTMessageSetupReturnStage<TMessage> : BuilderStageBase
{
    private readonly RequestToTypeBinder<TMessage> binder;
    private readonly FluentApiGroupRegistration fluentApiGroup;
    private readonly FluentApiMessageRegistration fluentApiMessage;

    public FluentTMessageSetupReturnStage(IServiceCollection services,
        FluentApiMessageRegistration fluentApiMessage,
        FluentApiGroupRegistration fluentApiGroup) : base(services)
    {
        this.fluentApiMessage = fluentApiMessage;
        this.fluentApiGroup = fluentApiGroup;
        string name = typeof(TMessage).Name;
        binder = new RequestToTypeBinder<TMessage>();
    }

    public FluentSetupDomainPostStage HandledBy<TReturn>(Func<RequestInput, TReturn> handler)
    {
        Task<object?> WrapperHandler(MessageRequest result, ILogger logger)
        {
            //requestResult.Start();
            var returnObject = handler.Invoke(result.RequestInput);
            return Task.FromResult<object?>(returnObject);
        }

        fluentApiMessage.RequestHandler = WrapperHandler;
        return new FluentSetupDomainPostStage(services, fluentApiGroup);
    }

#pragma warning disable SA1027
    // public FluentSetupDomainPostStage HandledBy(Func<TMessage, ILogger, object> handlerWithTReturn)
    // {
    // 	object? wrapperHandler(MessageRequest result, ILogger logger)
    // 	{
    // 		var message = binder.CreateMessage(result.Request);
    // 		var returnObject = handlerWithTReturn.Invoke(message, logger);
    // 		ReturnObjectHelper.CheckHandlerReturnType(returnObject, fluentApiMessage.ResponseRunTimeType!);
    // 		return returnObject!;
    // 	}
    // 	fluentApiMessage.RequestHandler = wrapperHandler;
    // 	return new FluentSetupDomainPostStage(services, fluentApiGroup);
    // }

    // public FluentSetupDomainPostStage HandledBy<TReturn>(Func<TMessage, ILogger, TReturn> handlerWithTReturn)
    // 	where TReturn : class
    // {
    // 	TReturn wrapperHandler(MessageRequest result, ILogger logger)
    // 	{
    // 		var message = binder.CreateMessage(result.Request);
    // 		var returnObject = handlerWithTReturn.Invoke(message, logger);
    // 		ReturnObjectHelper.CheckHandlerReturnType(returnObject, fluentApiMessage.ResponseRunTimeType!);
    // 		return returnObject!;
    // 	}
    // 	fluentApiMessage.RequestHandler = wrapperHandler;
    // 	return new FluentSetupDomainPostStage(services, fluentApiGroup);
    // }
#pragma warning restore SA1027

    public FluentSetupDomainPostStage HandledBy<TReturn>(Func<TMessage, ILogger, TReturn> handlerWithTReturn)
        where TReturn : class
    {
        Task<object?> WrapperHandler(MessageRequest result, ILogger logger)
        {
            var message = binder.CreateMessage(result.RequestInput);
            var returnObject = handlerWithTReturn.Invoke(message, logger);
            ReturnObjectHelper.CheckHandlerReturnType(returnObject, fluentApiMessage.ResponseRunTimeType!);
            return Task.FromResult<object?>(returnObject);
        }

        ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, WrapperHandler);
        return new FluentSetupDomainPostStage(services, fluentApiGroup);
    }
}
