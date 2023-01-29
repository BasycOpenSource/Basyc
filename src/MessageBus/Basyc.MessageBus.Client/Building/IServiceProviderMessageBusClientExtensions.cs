using Basyc.MessageBus.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

public static class ServiceProviderMessageBusClientExtensions
{
	public static Task StartBasycMessageBusClient(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
	{
		var busClient = serviceProvider.GetRequiredService<IObjectMessageBusClient>();
		return busClient.StartAsync(cancellationToken);
	}
}
