using Basyc.MessageBus.Manager.Infrastructure.Building;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.MessageBus.Manager.Infrastructure
{
	public class TypedProviderBuilder
    {
        public IServiceCollection services;

        public TypedProviderBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public TypedProviderBuilder RegisterDomain(Action<TypedDomainSettings> settingsAction)
        {
            services.Configure<TypedDomainProviderOptions>(options =>
            {
                var settings = new TypedDomainSettings();
                settingsAction(settings);
                options.TypedDomainOptions.Add(settings);
            });
            return this;
        }

        public SetupTypeFormattingStage ChangeFormatting()
        {
            return new SetupTypeFormattingStage(services);
        }
    }
}