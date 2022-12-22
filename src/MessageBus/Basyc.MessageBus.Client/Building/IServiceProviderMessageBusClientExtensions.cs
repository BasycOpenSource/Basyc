using Basyc.MessageBus.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder;

public static class IServiceProviderMessageBusClientExtensions
{
    public static Task StartBasycMessageBusClient(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var busClient = serviceProvider.GetRequiredService<IObjectMessageBusClient>();
        return busClient.StartAsync(cancellationToken);
    }
}
