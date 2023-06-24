using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.HandledByStages;

public class FluentSetupNoReturnHandledByStage : BuilderStageBase
{
    private readonly FluentApiGroupRegistration fluentApiGroup;
    private readonly FluentApiMessageRegistration fluentApiMessage;

    public FluentSetupNoReturnHandledByStage(IServiceCollection services, FluentApiMessageRegistration fluentApiMessage, FluentApiGroupRegistration fluentApiGroup)
        : base(services)
    {
        this.fluentApiMessage = fluentApiMessage;
        this.fluentApiGroup = fluentApiGroup;
    }

    public FluentSetupDomainPostStage HandledBy(Action<ILogger> handler)
    {
        Task<object?> HandlerWrapper(MessageRequest requestResult, ILogger logger)
        {
            using var act = logger.StartActivity("Invoking handler");
            handler.Invoke(logger);
            act.Stop();
            return Task.FromResult<object?>(null);
        }

        ReturnStageHelper.RegisterMessageRegistration(Services, fluentApiGroup, fluentApiMessage, HandlerWrapper);
        return new FluentSetupDomainPostStage(Services, fluentApiGroup);
    }

    public FluentSetupDomainPostStage HandledBy(Action<RequestInput, ILogger> handler)
    {
        Task<object?> HandlerWrapper(MessageRequest requestResult, ILogger logger)
        {
            using var act = logger.StartActivity("Invoking handler");
            handler.Invoke(requestResult.RequestInput, logger);
            act.Stop();
            return Task.FromResult<object?>(null);
        }

        ReturnStageHelper.RegisterMessageRegistration(Services, fluentApiGroup, fluentApiMessage, HandlerWrapper);
        return new FluentSetupDomainPostStage(Services, fluentApiGroup);
    }

    public FluentSetupDomainPostStage HandledBy(Func<RequestInput, ILogger, Task> handler)
    {
        async Task<object?> HandlerWrapper(MessageRequest requestResult, ILogger logger)
        {
            using var act = logger.StartActivity("Invoking handler");
            await handler.Invoke(requestResult.RequestInput, logger);
            act.Stop();
            return Task.FromResult<object?>(null);
        }

        ReturnStageHelper.RegisterMessageRegistration(Services, fluentApiGroup, fluentApiMessage, HandlerWrapper);
        return new FluentSetupDomainPostStage(Services, fluentApiGroup);
    }

    public FluentSetupDomainPostStage HandledBy(Func<ILogger, Task> handler)
    {
        async Task<object?> HandlerWrapper(MessageRequest requestResult, ILogger logger)
        {
            using var act = logger.StartActivity("Invoking handler");
            await handler.Invoke(logger);
            act.Stop();
            return Task.FromResult<object?>(null);
        }

        ReturnStageHelper.RegisterMessageRegistration(Services, fluentApiGroup, fluentApiMessage, HandlerWrapper);
        return new FluentSetupDomainPostStage(Services, fluentApiGroup);
    }
}
