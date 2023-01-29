using Microsoft.Extensions.Logging.Abstractions;

namespace Basyc.Repositories.EF.Tests.Repositories;

public class TestUnitOfWork : IUnitOfWork
{
	private readonly TestDbContext dbContext;

	public TestUnitOfWork(TestDbContext dbContext)
	{
		People = new PersonEfCrudRepository(dbContext, NullLogger<PersonEfCrudRepository>.Instance);
		Cars = new CarEfCrudRepository(dbContext, NullLogger<CarEfCrudRepository>.Instance);
		this.dbContext = dbContext;
	}

	public PersonEfCrudRepository People { get; }
	public CarEfCrudRepository Cars { get; }

	public void Save()
	{
		dbContext.SaveChanges();
	}

	public Task SaveAsync(CancellationToken cancellationToken = default)
	{
		return dbContext.SaveChangesAsync(cancellationToken);
	}

	public void Dispose()
	{
		dbContext.Dispose();
	}
}
