using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.Repositories.EF;

public class EFMigrationStartupFilter<TDbContext> : IStartupFilter where TDbContext : DbContext
{
	public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
	{
		return (app) =>
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
				db.Database.Migrate();
			}

			next(app);
		};
	}
}