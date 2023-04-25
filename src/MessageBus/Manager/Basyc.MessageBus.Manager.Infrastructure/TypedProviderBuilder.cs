using Basyc.MessageBus.Manager.Infrastructure.Building;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure;

public class TypedProviderBuilder
{
    public TypedProviderBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; init; }

    public TypedProviderBuilder RegisterDomain(Action<TypedDomainSettings> settingsAction)
    {
        Services.Configure<TypedDomainProviderOptions>(options =>
        {
            var settings = new TypedDomainSettings();
            settingsAction(settings);
            options.TypedDomainOptions.Add(settings);
        });
        return this;
    }

    public SetupTypeFormattingStage ChangeFormatting() => new(Services);
}
