using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Reflection;
using Throw;

// ReSharper disable InconsistentNaming
#pragma warning disable SA1402

namespace Basyc.Repositories.EF;

public abstract class EfAsyncInstantCrudRepositoryBase<TEntity, TEntityId, TModel, TModelId> : EfRepositoryBase<TEntity, TModel>,
    IAsyncInstantCrudRepository<TModel, TModelId>
    where TModel : class
    where TEntity : class, new()
    where TModelId : notnull
{
    private static bool isInitialized;

    protected EfAsyncInstantCrudRepositoryBase(DbContext dbContext,
        Expression<Func<TEntity, TEntityId>> entityIdPropertyNameSelector,
        Expression<Func<TModel, TModelId>> modelIdPropertyNameSelector,
        ILogger<EfAsyncInstantCrudRepositoryBase<TEntity, TEntityId, TModel, TModelId>> logger)
        : base(dbContext, logger)
    {
        if (isInitialized is false)
        {
            var imodeldPropertyExpression = modelIdPropertyNameSelector.Body as MemberExpression;
            imodeldPropertyExpression.ThrowIfNull();
            var modelPropertyInfo = imodeldPropertyExpression.Member as PropertyInfo;
            modelPropertyInfo.ThrowIfNull();
            ModelIdGetter = (Func<TModel, TModelId>)Delegate.CreateDelegate(typeof(Func<TModel, TModelId>), modelPropertyInfo.GetGetMethod()!);
            ModelIdSetter = (Action<TModel, TModelId>)Delegate.CreateDelegate(typeof(Action<TModel, TModelId>), modelPropertyInfo.GetSetMethod()!);

            var entityIdPropertyExpression = entityIdPropertyNameSelector.Body as MemberExpression;
            entityIdPropertyExpression.ThrowIfNull();
            var entityPropertyInfo = entityIdPropertyExpression.Member as PropertyInfo;
            entityPropertyInfo.ThrowIfNull();
            EntityIdGetter = (Func<TEntity, TEntityId>)Delegate.CreateDelegate(typeof(Func<TEntity, TEntityId>), entityPropertyInfo.GetGetMethod()!);
            EntityIdSetter = (Action<TEntity, TEntityId>)Delegate.CreateDelegate(typeof(Action<TEntity, TEntityId>), entityPropertyInfo.GetSetMethod()!);
        }

        isInitialized = true;
    }

    protected static Func<TEntity, TEntityId> EntityIdGetter { get; private set; } = null!;

    protected static Action<TEntity, TEntityId> EntityIdSetter { get; private set; } = null!;

    protected static Func<TModel, TModelId> ModelIdGetter { get; private set; } = null!;

    protected static Action<TModel, TModelId> ModelIdSetter { get; private set; } = null!;

    public async Task<TModel> InstaAddAsync(TModel model)
    {
        var entity = ToEntity(model);
        entity.ThrowIfNull();
        DbContext.Add(entity);
        await DbContext.SaveChangesAsync();
        var entityId = EntityIdGetter(entity);
        ModelIdSetter(model, ToModelId(entityId));
        return model;
    }

    public async Task<Dictionary<TModelId, TModel>> GetAllAsync()
    {
        var models = await DbContext.Set<TEntity>()
            .AsQueryable()
            .AsNoTracking()
            .AsSplitQuery()
            .ToDictionaryAsync(entity => ToModelId(EntityIdGetter(entity)), entity =>
            {
                var model = ToModel(entity);
                model.ThrowIfNull();
                return model!;
            });
        return models;
    }

    public async Task<TModel?> GetAsync(TModelId id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(ToEntityId(id)) ?? throw new InvalidOperationException($"Can't find entity with id: '{id}'");

        return ToModel(entity);
    }

    public async Task<TModel?> TryGetAsync(TModelId id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(ToEntityId(id));
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
        var modelId = ModelIdGetter(model);
        TEntity updatetedEntity;

        var oldEntityEntry = DbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => EntityIdGetter(x.Entity)!.Equals(modelId));
        if (oldEntityEntry is not null)
        {
            oldEntityEntry.CurrentValues.SetValues(entityToUpdate);
            updatetedEntity = oldEntityEntry.Entity;
        }
        else
        {
            updatetedEntity = DbContext.Update(entityToUpdate).Entity;
        }

        await DbContext.SaveChangesAsync();
        var updatedModel = ToModel(updatetedEntity);
        updatedModel.ThrowIfNull();
        return updatedModel;
    }

    public async Task InstaRemoveAsync(TModelId id)
    {
        var oldEntry = DbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => ToModelId(EntityIdGetter(x.Entity)).Equals(id));
        if (oldEntry != null)
        {
            DbContext.Set<TEntity>().Remove(oldEntry.Entity);
        }
        else
        {
            var entityToRemove = new TEntity();
            EntityIdSetter(entityToRemove, ToEntityId(id));
            DbContext.Set<TEntity>().Remove(entityToRemove);
        }

        await DbContext.SaveChangesAsync();
    }

    protected abstract TModelId ToModelId(TEntityId id);

    protected abstract TEntityId ToEntityId(TModelId id);
}

public abstract class EfInstantCrudRepositoryBase<TEntity, TModelId, TModel> : EfAsyncInstantCrudRepositoryBase<TEntity, TModelId, TModel, TModelId>
    where TModel : class
    where TEntity : class, new()
    where TModelId : notnull
{
    protected EfInstantCrudRepositoryBase(DbContext dbContext,
        Expression<Func<TEntity, TModelId>> entityIdSelector,
        Expression<Func<TModel, TModelId>> modelIdSelector,
        ILogger<EfInstantCrudRepositoryBase<TEntity, TModelId, TModel>> logger)
        : base(dbContext,
            entityIdSelector,
            modelIdSelector,
            logger)
    {
    }

    protected override TModelId ToEntityId(TModelId id) => id;

    protected override TModelId ToModelId(TModelId id) => id;
}

/// <summary>
///     Use when Model class is the same as Entity class.
/// </summary>
public abstract class EfInstantCrudRepositoryBase<TModel, TModelKey> : EfInstantCrudRepositoryBase<TModel, TModelKey, TModel>
    where TModel : class, new() where TModelKey : notnull
{
    protected EfInstantCrudRepositoryBase(DbContext dbContext,
        Expression<Func<TModel, TModelKey>> modelIdPropertyNameSelector,
        ILogger<EfInstantCrudRepositoryBase<TModel, TModelKey>> logger) : base(dbContext, modelIdPropertyNameSelector, modelIdPropertyNameSelector, logger)
    {
    }

    protected override TModel ToEntity(TModel model) => model;

    protected override TModel ToModel(TModel entity) =>
        //logger.LogInformation($"{nameof(ToModel)} called on {dbContext.GetType().Name}");
        entity;
}
