using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Throw;

namespace Basyc.Repositories.EF;

public class KonterDbContextBase<TDbContextImplementation> : DbContext, IDesignTimeDbContextFactory<TDbContextImplementation>
	where TDbContextImplementation : KonterDbContextBase<TDbContextImplementation>
{
	public KonterDbContextBase(DbContextOptions<TDbContextImplementation> options) : base(options)
	{
	}

	public virtual TDbContextImplementation CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json")
			.Build();
		var builder = new DbContextOptionsBuilder<TDbContextImplementation>();
		var connectionString = configuration.GetConnectionString(nameof(TDbContextImplementation));
		//builder.UseSqlServer(connectionString);

		var constructor = typeof(TDbContextImplementation).GetConstructor(new[] { typeof(DbContextOptions<TDbContextImplementation>) });
		constructor.ThrowIfNull();
		return (TDbContextImplementation)constructor.Invoke(new object[] { builder.Options });
	}
}
