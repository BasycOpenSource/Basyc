using Basyc.MessageBus.Manager.Presentation.BlazorLibrary;
using MudBlazor.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesBusMangerExtensions
{
	public static void AddBasycBusManagerBlazorUi(this IServiceCollection services)
	{
		services.AddMudServices();
		services.AddSingleton<BusManagerJsInterop>();
	}
}
