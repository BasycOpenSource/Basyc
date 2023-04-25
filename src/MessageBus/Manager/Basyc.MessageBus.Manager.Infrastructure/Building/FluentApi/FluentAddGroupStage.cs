using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentAddGroupStage : BuilderStageBase
{
    public FluentAddGroupStage(IServiceCollection services) : base(services)
    {
    }

    public FluentAddMessageStage InGroup(string groupName)
    {
        var newGroup = new FluentApiGroupRegistration { Name = groupName };
        Services.Configure<FluentApiDomainInfoProviderOptions>(x => x.GroupRegistrations.Add(newGroup));
        return new FluentAddMessageStage(Services, newGroup);
    }
}
