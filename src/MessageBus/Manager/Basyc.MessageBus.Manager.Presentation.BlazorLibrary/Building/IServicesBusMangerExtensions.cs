using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message;
using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Pages.Message.Sidebar;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Building;

public static class ServicesBusMangerExtensions
{
	public static void AddBasycBusManagerBlazorUi(this IServiceCollection services)
	{
		services.AddMudServices();
		services.AddSingleton<BusManagerJsInterop>();
		//TODO Automatically register viewmodels
		services.AddTransient<SidebarHistoryViewModel>();
		services.AddTransient<MessagePageViewModel>();
		services.AddTransient<SidebarHistoryItemViewModel>();

	}
}
