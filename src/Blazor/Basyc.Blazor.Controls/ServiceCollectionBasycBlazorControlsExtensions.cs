using System.Runtime.Versioning;
using Basyc.Blazor.Controls.Interops;

namespace Microsoft.Extensions.DependencyInjection;
[SupportedOSPlatform("browser")]
public static class ServiceCollectionBasycBlazorControlsExtensions
{
    public static IServiceCollection AddBasycBlazorControls(this IServiceCollection services)
    {
        services.AddSingleton<TooltipJsInterop>();
        services.AddBlazorJavaScriptInterop();
        return services;
    }
}
