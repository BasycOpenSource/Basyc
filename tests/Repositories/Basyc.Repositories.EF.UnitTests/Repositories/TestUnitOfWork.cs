using Microsoft.Extensions.Logging.Abstractions;

namespace Basyc.Repositories.EF.Tests.Repositories
{
    public class TestUnitOfWork : IUnitOfWork
    {
        private readonly TestDbContext dbContext;

        public TestUnitOfWork(TestDbContext dbContext)
        {
            People = new PersonEFCrudRepository(dbContext, NullLogger<PersonEFCrudRepository>.Instance);
            Cars = new CarEFCrudRepository(dbContext, NullLogger<CarEFCrudRepository>.Instance);
            this.dbContext = dbContext;
        }

        public PersonEFCrudRepository People { get; }
        public CarEFCrudRepository Cars { get; }

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
}