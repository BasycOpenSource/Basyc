using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Reflection;
using Throw;
// ReSharper disable InconsistentNaming
#pragma warning disable SA1402
#pragma warning disable SA1401
#pragma warning disable CA2211

namespace Basyc.Repositories.EF;

public abstract class EfAsyncInstantCrudRepositoryBase<TEntity, TEntityId, TModel, TModelId> : EfRepositoryBase<TEntity, TModel>,
    IAsyncInstantCrudRepository<TModel, TModelId>
    where TModel : class
    where TEntity : class, new()
    where TModelId : notnull
{
    protected static Func<TEntity, TEntityId> entityIdGetter = null!;

    protected static Action<TEntity, TEntityId> entityIdSetter = null!;

    protected static Func<TModel, TModelId> modelIdGetter = null!;

    protected static Action<TModel, TModelId> modelIdSetter = null!;

    private static bool isInitialized;

    public EfAsyncInstantCrudRepositoryBase(DbContext dbContext,
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
        DbContext.Add(entity);
        await DbContext.SaveChangesAsync();
        var entityId = entityIdGetter(entity);
        modelIdSetter(model, ToModelId(entityId));
        return model;
    }

    public async Task<Dictionary<TModelId, TModel>> GetAllAsync()
    {
        var models = await DbContext.Set<TEntity>()
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

    public async Task<TModel?> GetAsync(TModelId id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(ToEntityId(id));

        if (entity == null)
        {
            throw new InvalidOperationException($"Can't find entity with id: '{id}'");
        }

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
        var modelId = modelIdGetter(model);
        TEntity updatetedEntity;

        var oldEntityEntry = DbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => entityIdGetter(x.Entity)!.Equals(modelId));
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
        var oldEntry = DbContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => ToModelId(entityIdGetter(x.Entity)).Equals(id));
        if (oldEntry != null)
        {
            DbContext.Set<TEntity>().Remove(oldEntry.Entity);
        }
        else
        {
            var entityToRemove = new TEntity();
            entityIdSetter(entityToRemove, ToEntityId(id));
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
    public EfInstantCrudRepositoryBase(DbContext dbContext,
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
