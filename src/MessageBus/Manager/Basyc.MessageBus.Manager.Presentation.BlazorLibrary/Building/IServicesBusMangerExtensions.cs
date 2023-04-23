namespace Basyc.MessageBus.Manager.Presentation.BlazorLibrary.Building;

public static class ServicesBusMangerExtensions
{
    public static void AddBasycBusManagerBlazorUi(this IServiceCollection services)
    {
        BasycReactiveUi.Fix();
        services.AddMudServices();
        services.AddSingleton<BusManagerJsInterop>();
        services.AddSingleton<NavigationService>();
        services.RegisterViewModels();
        services.AddBasycBlazorControls();
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
