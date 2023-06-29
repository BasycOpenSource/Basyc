using System.Runtime.Versioning;
using Basyc.Blazor.Interops;

namespace Microsoft.Extensions.DependencyInjection;
[SupportedOSPlatform("browser")]
public static class ServiceCollectionInteropsExtensions
{
    public static IServiceCollection AddInterops(this IServiceCollection services)
    {
        services.AddSingleton<ScrollJsInterop>();
        services.AddSingleton<ElementJsInterop>();
        services.AddSingleton<PromptInterop>();
        return services;
    }
}
