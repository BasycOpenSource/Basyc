using Basyc.MessageBus.HttpProxy.Server.Asp.SignalR;
using Basyc.MessageBus.HttpProxy.Shared.SignalR;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationSignalRProxyExtensions
{
	public static WebApplication MapBasycSignalRMessageBusProxy(this WebApplication webApp, string hubPattern = SignalRConstants.ProxyClientHubPattern)
	{
		webApp.MapHub<ProxyClientHub>(hubPattern);
		return webApp;
	}
}
