namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionBasycBlazorControlsExtensions
{
    public static IServiceCollection AddBasycBlazorControls(this IServiceCollection services)
    {
        services.AddSingleton<TooltipJsInterop>();
        services.AddSingleton<ScrollJsInterop>();
        services.AddSingleton<ElementJsInterop>();
        return services;
    }
}
