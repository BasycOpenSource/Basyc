using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentAddMessageStage : BuilderStageBase
{
    private readonly FluentApiGroupRegistration fluentApiGroup;

    public FluentAddMessageStage(IServiceCollection services, FluentApiGroupRegistration fluentApiGroup) : base(services)
    {
        this.fluentApiGroup = fluentApiGroup;
    }

    public FluentSetupMessageStage AddMessage(string messageDisplayName)
    {
        var newMessage = new FluentApiMessageRegistration
        {
            MessageDisplayName = messageDisplayName,
            MessageType = MessageType.Generic
        };
        fluentApiGroup.InProgressMessages.Add(newMessage);
        return new FluentSetupMessageStage(Services, newMessage, fluentApiGroup);
    }
}
