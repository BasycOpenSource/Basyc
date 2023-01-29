using Microsoft.EntityFrameworkCore;

namespace Basyc.Repositories.EF.Tests;

public class TestDbContext : DbContext
{
	public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
	{
	}

	public DbSet<PersonEntity> People { get; set; } = null!;
	public DbSet<CarEntity> Cars { get; set; } = null!;
}
