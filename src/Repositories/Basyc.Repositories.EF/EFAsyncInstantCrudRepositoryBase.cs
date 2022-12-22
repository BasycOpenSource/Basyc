using Basyc.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories.EF;

public abstract class EFAsyncInstantCrudRepositoryBase<TEntity, TEntityId, TModel, TModelId> : EFRepositoryBase<TEntity, TModel>, IAsyncInstantCrudRepository<TModel, TModelId>
	where TModel : class
	where TEntity : class, new()
{
	private static bool isInitialized = false;

	protected static Func<TEntity, TEntityId> EntityIdGetter { get; private set; }
	protected static Action<TEntity, TEntityId> EntityIdSetter { get; private set; }
	protected static Func<TModel, TModelId> ModelIdGetter { get; private set; }
	protected static Action<TModel, TModelId> ModelIdSetter { get; private set; }

	public EFAsyncInstantCrudRepositoryBase(DbContext dbContext, Expression<Func<TEntity, TEntityId>> entityIdPropertyNameSelector, Expression<Func<TModel, TModelId>> modelIdPropertyNameSelector, ILogger<EFAsyncInstantCrudRepositoryBase<TEntity, TEntityId, TModel, TModelId>> logger) : base(dbContext, logger)
	{
		if (isInitialized is false)
		{
			var ImodeldPropertyExpression = modelIdPropertyNameSelector.Body as MemberExpression;
			var modelPropertyInfo = ImodeldPropertyExpression.Member as PropertyInfo;
			ModelIdGetter = (Func<TModel, TModelId>)Delegate.CreateDelegate(typeof(Func<TModel, TModelId>), modelPropertyInfo.GetGetMethod());
			ModelIdSetter = (Action<TModel, TModelId>)Delegate.CreateDelegate(typeof(Action<TModel, TModelId>), modelPropertyInfo.GetSetMethod());

			var entityIdPropertyExpression = entityIdPropertyNameSelector.Body as MemberExpression;
			var entityPropertyInfo = entityIdPropertyExpression.Member as PropertyInfo;
			EntityIdGetter = (Func<TEntity, TEntityId>)Delegate.CreateDelegate(typeof(Func<TEntity, TEntityId>), entityPropertyInfo.GetGetMethod());
			EntityIdSetter = (Action<TEntity, TEntityId>)Delegate.CreateDelegate(typeof(Action<TEntity, TEntityId>), entityPropertyInfo.GetSetMethod());
		}

		isInitialized = true;
	}

	protected abstract TModelId ToModelId(TEntityId id);

	protected abstract TEntityId ToEntityId(TModelId id);

	public async Task<TModel> InstaAddAsync(TModel model)
	{
		var entity = ToEntity(model);
		dbContext.Add(entity);
		await dbContext.SaveChangesAsync();
		var entityId = EntityIdGetter(entity);
		ModelIdSetter(model, ToModelId(entityId));
		return model;
	}

	public async Task<Dictionary<TModelId, TModel>> GetAllAsync()
	{
		var models = await dbContext.Set<TEntity>()
			.AsQueryable()
			.AsNoTracking()
			.AsSplitQuery()
			.ToDictionaryAsync(entity => ToModelId(EntityIdGetter(entity)), entity => ToModel(entity));
		return models;
	}

	public async Task<TModel> GetAsync(TModelId modelId)
	{
		var entity = await dbContext.Set<TEntity>().FindAsync(ToEntityId(modelId));

		if (entity == null)
			throw new InvalidOperationException($"Can't find entity with id: '{modelId}'");

		return ToModel(entity);
	}

	public async Task<TModel> TryGetAsync(TModelId modelId)
	{
		var entity = await dbContext.Set<TEntity>().FindAsync(ToEntityId(modelId));
		return ToModel(entity);
	}

	public async Task<TModel> InstaUpdateAsync(TModel model)
	{
		var entityToUpdate = ToEntity(model);
		var modelId = ModelIdGetter(model);
		TEntity updatetedEntity;

		var oldEntityEntry = dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => EntityIdGetter(x.Entity).Equals(modelId));
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
		return updatedModel;
	}

	public async Task InstaRemoveAsync(TModelId modelId)
	{
		var oldEntry = dbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => ToModelId(EntityIdGetter(x.Entity)).Equals(modelId));
		if (oldEntry != null)
		{
			dbContext.Set<TEntity>().Remove(oldEntry.Entity);
		}
		else
		{
			var entityToRemove = new TEntity();
			EntityIdSetter(entityToRemove, ToEntityId(modelId));
			dbContext.Set<TEntity>().Remove(entityToRemove);
		}

		await dbContext.SaveChangesAsync();
	}
}

public abstract class EFInstantCrudRepositoryBase<TEntity, TModelId, TModel> : EFAsyncInstantCrudRepositoryBase<TEntity, TModelId, TModel, TModelId>
	where TModel : class
	where TEntity : class, new()
{
	public EFInstantCrudRepositoryBase(DbContext dbContext, Expression<Func<TEntity, TModelId>> entityIdSelector, Expression<Func<TModel, TModelId>> modelIdSelector, ILogger<EFInstantCrudRepositoryBase<TEntity, TModelId, TModel>> logger) : base(dbContext, entityIdSelector, modelIdSelector, logger)
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
/// Use when Model class is the same as Entity class
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TModelKey"></typeparam>
public abstract class EFInstantCrudRepositoryBase<TModel, TModelKey> : EFInstantCrudRepositoryBase<TModel, TModelKey, TModel>
   where TModel : class, new()
{
	protected EFInstantCrudRepositoryBase(DbContext dbContext, Expression<Func<TModel, TModelKey>> modelIdPropertyNameSelector, ILogger<EFInstantCrudRepositoryBase<TModel, TModelKey>> logger) : base(dbContext, modelIdPropertyNameSelector, modelIdPropertyNameSelector, logger)
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