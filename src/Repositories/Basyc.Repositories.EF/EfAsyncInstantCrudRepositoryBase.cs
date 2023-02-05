using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Reflection;
using Throw;

namespace Basyc.Repositories.EF;

public abstract class EfAsyncInstantCrudRepositoryBase<TEntity, TEntityId, TModel, TModelId> : EfRepositoryBase<TEntity, TModel>,
	IAsyncInstantCrudRepository<TModel, TModelId>
	where TModel : class
	where TEntity : class, new()
	where TModelId : notnull
{
	private static bool isInitialized;

	protected static Func<TEntity, TEntityId> entityIdGetter = null!;
	protected static Action<TEntity, TEntityId> entityIdSetter = null!;
	protected static Func<TModel, TModelId> modelIdGetter = null!;
	protected static Action<TModel, TModelId> modelIdSetter = null!;

	public EfAsyncInstantCrudRepositoryBase(DbContext dbContext, Expression<Func<TEntity, TEntityId>> entityIdPropertyNameSelector,
		Expression<Func<TModel, TModelId>> modelIdPropertyNameSelector, ILogger<EfAsyncInstantCrudRepositoryBase<TEntity, TEntityId, TModel, TModelId>> logger)
		: base(dbContext, logger)
	{
		if (isInitialized is false)
		{
			var ImodeldPropertyExpression = modelIdPropertyNameSelector.Body as MemberExpression;
			ImodeldPropertyExpression.ThrowIfNull();
			var modelPropertyInfo = ImodeldPropertyExpression.Member as PropertyInfo;
			modelPropertyInfo.ThrowIfNull();
			modelIdGetter = (Func<TModel, TModelId>)Delegate.CreateDelegate(typeof(Func<TModel, TModelId>), modelPropertyInfo.GetGetMethod()!);
			modelIdSetter = (Action<TModel, TModelId>)Delegate.CreateDelegate(typeof(Action<TModel, TModelId>), modelPropertyInfo.GetSetMethod()!);

			var entityIdPropertyExpression = entityIdPropertyNameSelector.Body as MemberExpression;
			entityIdPropertyExpression.ThrowIfNull();
			var entityPropertyInfo = entityIdPropertyExpression.Member as PropertyInfo;
			entityPropertyInfo.ThrowIfNull();
			entityIdGetter = (Func<TEntity, TEntityId>)Delegate.CreateDelegate(typeof(Func<TEntity, TEntityId>), entityPropertyInfo.GetGetMethod()!);
			entityIdSetter = (Action<TEntity, TEntityId>)Delegate.CreateDelegate(typeof(Action<TEntity, TEntityId>), entityPropertyInfo.GetSetMethod()!);
		}

		isInitialized = true;
	}

	public async Task<TModel> InstaAddAsync(TModel model)
	{
		var entity = ToEntity(model);
		entity.ThrowIfNull();
		dbContext.Add(entity);
		await dbContext.SaveChangesAsync();
		var entityId = entityIdGetter(entity);
		modelIdSetter(model, ToModelId(entityId));
		return model;
	}

	public async Task<Dictionary<TModelId, TModel>> GetAllAsync()
	{
		var models = await dbContext.Set<TEntity>()
			.AsQueryable()
			.AsNoTracking()
			.AsSplitQuery()
			.ToDictionaryAsync(entity => ToModelId(entityIdGetter(entity)), entity =>
			{
				var model = ToModel(entity);
				model.ThrowIfNull();
				return model!;
			});
		return models;
	}

	public async Task<TModel?> GetAsync(TModelId modelId)
	{
		var entity = await dbContext.Set<TEntity>().FindAsync(ToEntityId(modelId));

		if (entity == null)
		{
			throw new InvalidOperationException($"Can't find entity with id: '{modelId}'");
		}

		return ToModel(entity);
	}

	public async Task<TModel?> TryGetAsync(TModelId modelId)
	{
		var entity = await dbContext.Set<TEntity>().FindAsync(ToEntityId(modelId));
		if (entity == null)
		{
			return null;
		}

		return ToModel(entity);
	}

	public async Task<TModel> InstaUpdateAsync(TModel model)
	{
		var entityToUpdate = ToEntity(model);
		entityToUpdate.ThrowIfNull();
		var modelId = modelIdGetter(model);
		TEntity updatetedEntity;

		var oldEntityEntry = dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => entityIdGetter(x.Entity)!.Equals(modelId));
		if (oldEntityEntry is not null)
		{
			oldEntityEntry.CurrentValues.SetValues(entityToUpdate);
			updatetedEntity = oldEntityEntry.Entity;
		}
		else
		{
			updatetedEntity = dbContext.Update(entityToUpdate).Entity;
		}

		await dbContext.SaveChangesAsync();
		var updatedModel = ToModel(updatetedEntity);
		updatedModel.ThrowIfNull();
		return updatedModel;
	}

	public async Task InstaRemoveAsync(TModelId modelId)
	{
		var oldEntry = dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => ToModelId(entityIdGetter(x.Entity)).Equals(modelId));
		if (oldEntry != null)
		{
			dbContext.Set<TEntity>().Remove(oldEntry.Entity);
		}
		else
		{
			var entityToRemove = new TEntity();
			entityIdSetter(entityToRemove, ToEntityId(modelId));
			dbContext.Set<TEntity>().Remove(entityToRemove);
		}

		await dbContext.SaveChangesAsync();
	}

	protected abstract TModelId ToModelId(TEntityId id);

	protected abstract TEntityId ToEntityId(TModelId id);
}

public abstract class EfInstantCrudRepositoryBase<TEntity, TModelId, TModel> : EfAsyncInstantCrudRepositoryBase<TEntity, TModelId, TModel, TModelId>
	where TModel : class
	where TEntity : class, new()
	where TModelId : notnull
{
	public EfInstantCrudRepositoryBase(DbContext dbContext, Expression<Func<TEntity, TModelId>> entityIdSelector,
		Expression<Func<TModel, TModelId>> modelIdSelector, ILogger<EfInstantCrudRepositoryBase<TEntity, TModelId, TModel>> logger) : base(dbContext,
		entityIdSelector, modelIdSelector, logger)
	{
	}

	protected override TModelId ToEntityId(TModelId id)
	{
		return id;
	}

	protected override TModelId ToModelId(TModelId id)
	{
		return id;
	}
}

/// <summary>
///     Use when Model class is the same as Entity class
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TModelKey"></typeparam>
public abstract class EfInstantCrudRepositoryBase<TModel, TModelKey> : EfInstantCrudRepositoryBase<TModel, TModelKey, TModel>
	where TModel : class, new() where TModelKey : notnull
{
	protected EfInstantCrudRepositoryBase(DbContext dbContext, Expression<Func<TModel, TModelKey>> modelIdPropertyNameSelector,
		ILogger<EfInstantCrudRepositoryBase<TModel, TModelKey>> logger) : base(dbContext, modelIdPropertyNameSelector, modelIdPropertyNameSelector, logger)
	{
	}

	protected override TModel ToEntity(TModel model)
	{
		return model;
	}

	protected override TModel ToModel(TModel entity)
	{
		//logger.LogInformation($"{nameof(ToModel)} called on {dbContext.GetType().Name}");
		return entity;
	}
}
