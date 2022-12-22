using Basyc.MessageBus.Broker;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceProviderMessageBrokerServerExtensions
{
    public static Task StartMessageBrokerAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var broker = serviceProvider.GetRequiredService<IMessageBrokerServer>();
        return broker.StartAsync(cancellationToken);
    }
}
