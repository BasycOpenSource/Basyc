using Basyc.MessageBus.Manager.Presentation.BlazorLibrary;
using MudBlazor.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServicesBusMangerExtensions
{
    public static void AddBasycBusManagerBlazorUI(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddSingleton<BusManagerJSInterop>();
    }
}
