using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories.EF;

public static class IHostEFMigratorExtensions
{
	public static IHostBuilder MigrateDatabaseOnStart<TDbContext>(this IHostBuilder host)
		where TDbContext : DbContext
	{
		host.ConfigureServices(ConfigureMigartionServices<TDbContext>);
		return host;
	}

	private static void ConfigureMigartionServices<TDBContext>(HostBuilderContext context, IServiceCollection services)
		where TDBContext : DbContext
	{
		services.AddTransient<IStartupFilter, EFMigrationStartupFilter<TDBContext>>();
	}
}
