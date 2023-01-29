using Microsoft.Extensions.DependencyInjection;

namespace Basyc.Localizator.Abstraction;

public class LocalizationDependencyBuilder
{
	private readonly IServiceCollection services;

	public LocalizationDependencyBuilder(IServiceCollection services)
	{
		this.services = services;
	}

	public LocalizationDependencyBuilder AddStorage<TLocalizatorStorage>() where TLocalizatorStorage : class, ILocalizatorStorage
	{
		services.AddSingleton<ILocalizatorStorage, TLocalizatorStorage>();
		return this;
	}
}
