using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Basyc.Repositories.EF;

public abstract class EfRepositoryBase<TEntity, TModel> where TEntity : class
{
	private static bool isDbContextValidated;
	protected readonly DbContext dbContext;
	protected readonly ILogger<EfRepositoryBase<TEntity, TModel>> logger;

	public EfRepositoryBase(DbContext dbContext, ILogger<EfRepositoryBase<TEntity, TModel>> logger)
	{
		this.dbContext = dbContext;
		this.logger = logger;
		if (isDbContextValidated is false)
		{
			ValidateDbContext(dbContext);
			isDbContextValidated = true;
		}
	}

	/// <summary>
	///     Checks if generic DbContext contains a requiered Set<<see cref="TEntity" />>
	/// </summary>
	/// <param name="dbContext"></param>
	private void ValidateDbContext(DbContext dbContext)
	{
		logger.LogInformation($"Validating dbContext: {dbContext.GetType().Name}");
		dbContext.Set<TEntity>();
	}

	protected abstract TEntity? ToEntity(TModel model);

	protected abstract TModel? ToModel(TEntity entity);
}
