using Basyc.Blazor.Interops;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionInteropExtensions
{
    public static IServiceCollection AddBlazorJavaScriptInterop(this IServiceCollection services)
    {
        services.AddSingleton<ScrollJsInterop>();
        services.AddSingleton<ElementInterop>();
        //services.AddSingleton<PromptInterop>();
        return services;
    }
}
