namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.HandledByStages;

public class FluentSetupNoReturnHandledByStage : BuilderStageBase
{
    private readonly FluentApiGroupRegistration fluentApiGroup;
    private readonly FluentApiMessageRegistration fluentApiMessage;

    public FluentSetupNoReturnHandledByStage(IServiceCollection services, FluentApiMessageRegistration fluentApiMessage, FluentApiGroupRegistration fluentApiGroup) :
        base(services)
    {
        this.fluentApiMessage = fluentApiMessage;
        this.fluentApiGroup = fluentApiGroup;
    }

    private FluentSetupDomainPostStage HandledBy(RequestHandlerDelegate handler)
    {
        fluentApiMessage.RequestHandler = handler;
        return new FluentSetupDomainPostStage(services, fluentApiGroup);
    }

    // public FluentSetupDomainPostStage HandledBy(Action<RequestInput, ILogger> handler)
    // {
    // 	object? ToRequestContextAction(MessageRequest requestResult, ILogger logger)
    // 	{
    // 		using var act = logger.StartActivity("Invoking handler");
    // 		handler.Invoke(requestResult.Request, logger);
    // 		act.Stop();
    // 		return null;
    // 	}
    //
    // 	fluentApiMessage.RequestHandler = ToRequestContextAction;
    // 	return new FluentSetupDomainPostStage(services, fluentApiGroup);
    // }

    // public FluentSetupDomainPostStage HandledBy(Action<RequestInput, ILogger> handler)
    // {
    // 	object? handlerWrapper(MessageRequest requestResult, ILogger logger)
    // 	{
    // 		using var act = logger.StartActivity("Invoking handler");
    // 		handler.Invoke(requestResult.Request, logger);
    // 		act.Stop();
    // 		return null;
    // 	}
    //
    // 	ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, handlerWrapper);
    // 	return new FluentSetupDomainPostStage(services, fluentApiGroup);
    // }

    public FluentSetupDomainPostStage HandledBy(Action<ILogger> handler)
    {
        Task<object?> handlerWrapper(MessageRequest requestResult, ILogger logger)
        {
            using var act = logger.StartActivity("Invoking handler");
            handler.Invoke(logger);
            act.Stop();
            return Task.FromResult<object?>(null);
        }

        ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, handlerWrapper);
        return new FluentSetupDomainPostStage(services, fluentApiGroup);
    }

    public FluentSetupDomainPostStage HandledBy(Action<RequestInput, ILogger> handler)
    {
        Task<object?> handlerWrapper(MessageRequest requestResult, ILogger logger)
        {
            using var act = logger.StartActivity("Invoking handler");
            handler.Invoke(requestResult.RequestInput, logger);
            act.Stop();
            return Task.FromResult<object?>(null);
        }

        ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, handlerWrapper);
        return new FluentSetupDomainPostStage(services, fluentApiGroup);
    }

    public FluentSetupDomainPostStage HandledBy(Func<ILogger, Task> handler)
    {
        async Task<object?> handlerWrapper(MessageRequest requestResult, ILogger logger)
        {
            using var act = logger.StartActivity("Invoking handler");
            await handler.Invoke(logger);
            act.Stop();
            return Task.FromResult<object?>(null);
        }

        ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, handlerWrapper);
        return new FluentSetupDomainPostStage(services, fluentApiGroup);
    }
}
