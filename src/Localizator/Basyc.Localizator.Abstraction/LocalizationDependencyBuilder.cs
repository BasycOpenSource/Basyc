using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basyc.Localizator.Abstraction;

public class LocalizationDependencyBuilder
{
	private readonly IServiceCollection _servics;
	public LocalizationDependencyBuilder(IServiceCollection services)
	{
		_servics = services;
	}

	public LocalizationDependencyBuilder AddStorage<TLocalizatorStorage>() where TLocalizatorStorage : class, ILocalizatorStorage
	{
		_servics.AddSingleton<ILocalizatorStorage, TLocalizatorStorage>();
		return this;
	}
}
