using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories.EF;

public class KonterDbContextBase<TDbContextImplementation> : DbContext, IDesignTimeDbContextFactory<TDbContextImplementation>
	where TDbContextImplementation : KonterDbContextBase<TDbContextImplementation>
{
	public KonterDbContextBase(DbContextOptions<TDbContextImplementation> options) : base(options)
	{
	}

	public virtual TDbContextImplementation CreateDbContext(string[] args)
	{
		IConfigurationRoot configuration = new ConfigurationBuilder()
	 .SetBasePath(Directory.GetCurrentDirectory())
	 .AddJsonFile("appsettings.json")
	 .Build();
		DbContextOptionsBuilder<TDbContextImplementation> builder = new DbContextOptionsBuilder<TDbContextImplementation>();
		string connectionString = configuration.GetConnectionString(nameof(TDbContextImplementation));
		//builder.UseSqlServer(connectionString);

		var constructor = typeof(TDbContextImplementation).GetConstructor(new Type[] { typeof(DbContextOptions<TDbContextImplementation>) });
		return (TDbContextImplementation)constructor.Invoke(new object[] { builder.Options });
	}
}
