using Basyc.MessageBus.Manager.Application.Initialization;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure;

public static class MessageManagerBuilderTypedExtensions
{
    public static TypedProviderBuilder UseTypedProvider(this BusManagerApplicationBuilder managerBuilder)
    {
        managerBuilder.Services.AddSingleton<IMessageInfoProvider, TypedMessageInfoProvider>();

        return new TypedProviderBuilder(managerBuilder.Services);
    }
}
