using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupDomainPostStage : BuilderStageBase
{
    private readonly FluentApiGroupRegistration inProgressGroup;

    public FluentSetupDomainPostStage(IServiceCollection services, FluentApiGroupRegistration inProgressGroup) : base(services)
    {
        this.inProgressGroup = inProgressGroup;
    }

    public FluentSetupMessageStage AddMessage(string messageDisplayName) => new FluentAddMessageStage(Services, inProgressGroup).AddMessage(messageDisplayName);

    public FluentAddMessageStage AddDomain(string domainName) => new FluentAddGroupStage(Services).InGroup(domainName);
}
