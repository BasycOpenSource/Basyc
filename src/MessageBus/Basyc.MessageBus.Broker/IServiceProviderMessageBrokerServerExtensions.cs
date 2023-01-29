using Basyc.MessageBus.Broker;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceProviderMessageBrokerServerExtensions
{
	public static Task StartMessageBrokerAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
	{
		var broker = serviceProvider.GetRequiredService<IMessageBrokerServer>();
		return broker.StartAsync(cancellationToken);
	}
}
