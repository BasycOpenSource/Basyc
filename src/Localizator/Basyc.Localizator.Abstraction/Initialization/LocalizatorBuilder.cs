using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Abstraction.Initialization;

public class LocalizatorBuilder
{
	private readonly IServiceCollection services;

	public LocalizatorBuilder(IServiceCollection services)
	{
		this.services = services;
	}

	public void AddStorage<TLocalizatorStorage>() where TLocalizatorStorage : class, ILocalizatorStorage
	{
		services.AddSingleton<ILocalizatorStorage, TLocalizatorStorage>();
	}
}
