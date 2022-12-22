using Basyc.MessageBus.Manager.Application.Building.Stages.MessageRegistration;
using Basyc.MessageBus.Manager.Application.Initialization;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure;

public static class MessageManagerBuilderTypedExtensions
{
    public static TypedProviderBuilder UseTypedProvider(this BusManagerApplicationBuilder managerBuilder)
    {
        managerBuilder.services.AddSingleton<IDomainInfoProvider, TypedDomainProvider>();

        return new TypedProviderBuilder(managerBuilder.services);
    }
}
