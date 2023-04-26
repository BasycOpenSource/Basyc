using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Basyc.Repositories.EF;

public abstract class EfRepositoryBase<TEntity, TModel> where TEntity : class
{
    private static bool isDbContextValidated;

    protected EfRepositoryBase(DbContext dbContext, ILogger<EfRepositoryBase<TEntity, TModel>> logger)
    {
        DbContext = dbContext;
        Logger = logger;
        if (isDbContextValidated is false)
        {
            ValidateDbContext(dbContext);
            isDbContextValidated = true;
        }
    }

    protected ILogger<EfRepositoryBase<TEntity, TModel>> Logger { get; init; }

    protected DbContext DbContext { get; init; }

    protected abstract TEntity? ToEntity(TModel model);

    protected abstract TModel? ToModel(TEntity entity);

    /// <summary>
    ///     Checks if generic DbContext contains a required Set <typeparamref name="TEntity"/>.
    /// </summary>
    private void ValidateDbContext(DbContext dbContext)
    {
        Logger.LogInformation("Validating dbContext: {Name}", dbContext.GetType().Name);
        dbContext.Set<TEntity>();
    }
}
