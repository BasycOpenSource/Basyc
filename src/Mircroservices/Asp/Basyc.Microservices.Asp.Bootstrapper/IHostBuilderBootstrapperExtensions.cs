using Basyc.Asp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Basyc.MicroService.Asp.Bootstrapper;

public static class HostBuilderBootstrapperExtensions
{
	private static readonly Assembly entryAssembly = Assembly.GetEntryAssembly()!;

	public static MicroserviceBuilder<IHostBuilder> CreateMicroserviceBuilder<TStartup>(this IHostBuilder hostBuilder) where TStartup : class, IStartupClass
	{
		var builderServices = new ServiceCollection();

		hostBuilder.ConfigureWebHostDefaults(webBuilder =>
		{
			//Builder services needs to be manualy moved into web builder services
			webBuilder.ConfigureServices((s, aspServices) =>
			{
				foreach (var service in builderServices)
				{
					aspServices.Add(service);
				}
			});
			webBuilder.ConfigureAsp<TStartup>(entryAssembly.GetName()!.Name!);
		});

		return new MicroserviceBuilder<IHostBuilder>(builderServices, hostBuilder);
	}
}
