using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Localizator.Abstraction.Initialization;

public static class ServiceCollectionLocalizatorExtensions
{
    public static LocalizatorBuilder AddLocalizator(this IServiceCollection services)
    {
        services.AddSingleton<ILocalizationManager, LocalizationManager>();

        return new LocalizatorBuilder(services);
    }
}
