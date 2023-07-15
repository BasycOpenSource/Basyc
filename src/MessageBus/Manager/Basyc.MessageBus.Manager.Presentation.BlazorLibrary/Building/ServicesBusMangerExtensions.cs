using Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Shared.Navigation;
using Basyc.ReactiveUi;
using Excubo.Blazor.ScriptInjection;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Runtime.Versioning;

namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Building;
[SupportedOSPlatform("browser")]
public static class ServicesBusMangerExtensions
{
    public static void AddBasycBusManagerBlazorUi(this IServiceCollection services)
    {
        BasycReactiveUi.Fix();
        services.AddMudServices();
        services.AddScriptInjection();
        services.AddBasycBlazorControls();
        services.AddBlazorJavaScriptInterop();
        services.AddSingleton<BusManagerJsInterop>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.RegisterViewModels();
    }

    private static void RegisterViewModels(this IServiceCollection services)
    {
        var assembly = typeof(ServicesBusMangerExtensions).Assembly;
        var viewModelTypes = assembly.GetTypes().Where(x => x.Name.EndsWith("ViewModel") && x.IsAssignableTo(typeof(BasycReactiveViewModelBase)));
        foreach (var viewModelType in viewModelTypes)
        {
            services.AddTransient(viewModelType);
        }
    }
}
