using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basyc.Repositories.EF;

public static class HostEfMigratorExtensions
{
	public static IHostBuilder MigrateDatabaseOnStart<TDbContext>(this IHostBuilder host)
		where TDbContext : DbContext
	{
		host.ConfigureServices(ConfigureMigartionServices<TDbContext>);
		return host;
	}

	private static void ConfigureMigartionServices<TDbContext>(HostBuilderContext context, IServiceCollection services)
		where TDbContext : DbContext
	{
		services.AddTransient<IStartupFilter, EfMigrationStartupFilter<TDbContext>>();
	}
}
